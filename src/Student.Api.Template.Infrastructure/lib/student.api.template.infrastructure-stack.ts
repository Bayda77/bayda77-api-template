import { Duration, RemovalPolicy, Stack } from "aws-cdk-lib";
import { AuthorizationType, CfnAuthorizer, Cors, IResource, LambdaIntegration, Period, RestApi } from "aws-cdk-lib/aws-apigateway";
import { Configuration } from "./configurations";
import { ISecurityGroup, ISubnet, IVpc, Subnet, Vpc } from "aws-cdk-lib/aws-ec2";
import { ArnPrincipal, Effect, PolicyStatement } from "aws-cdk-lib/aws-iam";
import { Rule, RuleTargetInput, Schedule } from "aws-cdk-lib/aws-events";
import { Construct } from "constructs";
import path = require("path");
import { Topic } from "aws-cdk-lib/aws-sns";
import { Code, Function, Runtime } from "aws-cdk-lib/aws-lambda";
import { LambdaFunction } from "aws-cdk-lib/aws-events-targets";
import { AttributeType, BillingMode, Table } from "aws-cdk-lib/aws-dynamodb";
import { SecurityGroup } from "@nutrien/ddc-cdk-lib";

const corsOptions = {
  allowOrigins: Cors.ALL_ORIGINS,
  allowMethods: Cors.ALL_METHODS,
  allowCredentials: true,
  allowHeaders: Cors.DEFAULT_HEADERS.concat(["Cache-Control", "Pragma"]),
  statusCode: 200,
};

class StudentTemplateStack extends Stack {
  protected zipFileLocation: string;
  protected config: Configuration;
  protected myVpc: IVpc;
  protected secretsPolicy: PolicyStatement;
  protected keepWarmRule: Rule;
  protected keepWarmCount: number = 0;
  protected mySecurityGroup: ISecurityGroup;
  protected api: RestApi;
  protected auth: CfnAuthorizer;
  protected name: string;
  protected mySubnets: ISubnet[];

  constructor(
    scope: Construct,
    id: string,
    apiName: string,
    config: Configuration
  ) {
    super(scope, id, { env: config.env });

    this.config = config;
    this.zipFileLocation = path.join(
      __dirname,
      "../../../dist/LambdaPackage.zip"
    );
    this.myVpc = Vpc.fromLookup(this, "nutrien-vpc", {
      vpcId: config.vpcId,
    });
    this.secretsPolicy = this.createPolicyStatementForSecrets();

    this.mySubnets = config.subnetIds.map(subnetId => Subnet.fromSubnetId(this, `${config.stackNamePrefix}-${subnetId}`, subnetId));
    this.mySecurityGroup = new SecurityGroup(this, `${id}-sg`, {
      description: `Custom security group for ${id} in student-api-template`,
      securityGroupName: `${config.studentName}-sg-${config.stageName}`,
      vpc: this.myVpc,
      allowAllOutbound: true
    })

    this.api = this.createApiGateway(apiName);
    this.auth = this.createAuthorizationFor(this.api);
  }

  protected createAuthorizationFor(api: RestApi) {
    const auth = new CfnAuthorizer(this, "APIGatewayAuthorizer", {
      name: "studentTemplate-authorizer",
      identitySource: "method.request.header.Authorization",
      providerArns: [this.config.providerArn],
      restApiId: api.restApiId,
      type: AuthorizationType.COGNITO,
    });
    return auth;
  }

  protected createApiGateway(name: string) {
    const api = new RestApi(this, name, {
      restApiName: name,
      deployOptions: {
        stageName: this.config.stageName,
      },
      defaultCorsPreflightOptions: corsOptions,
    });
    return api;
  }

  protected createSnsTopic(topicName: string, displayName: string) {
    return new Topic(this, topicName, {
      topicName: topicName,
      displayName: displayName,
    });
  }

  protected createPolicyStatementForSns(snsTopicArn: string) {
    return new PolicyStatement({
      effect: Effect.ALLOW,
      resources: [snsTopicArn],
      actions: ["sns:Publish"],
    });
  }

  protected createAccessPolicyForSns(userArn: string, sid: string, snsArn: string) {
    return new PolicyStatement({
      sid: sid,
      effect: Effect.ALLOW,
      principals: [new ArnPrincipal(userArn)],
      actions: [
        "sns:GetTopicAttributes",
        "sns:SetTopicAttributes",
        "sns:AddPermission",
        "sns:RemovePermission",
        "sns:DeleteTopic",
        "sns:Subscribe",
        "sns:ListSubscriptionsByTopic",
        "sns:Publish",
        "sns:Receive",
      ],
      resources: [snsArn],
    });
  }

  protected createLambda(id: string, handler: string, attachToVpc : boolean = true) {
    return new Function(this, id, {
      runtime: Runtime.DOTNET_6,
      handler: handler,
      code: Code.fromAsset(this.zipFileLocation),
      timeout: Duration.seconds(60),
      environment: { stageName: this.config.stageName },
      vpc: attachToVpc ? this.myVpc : undefined,
      vpcSubnets: attachToVpc ? {
        subnets: this.mySubnets,
      }: undefined,
      memorySize: 1024,
      securityGroups: [this.mySecurityGroup],
    });
  }

  protected createPolicyStatementForSecrets() {
    // see https://docs.aws.amazon.com/IAM/latest/UserGuide/list_awssecretsmanager.html
    const secretsPolicy = new PolicyStatement({
      effect: Effect.ALLOW,
    });
    secretsPolicy.addActions("secretsmanager:GetSecretValue");
    secretsPolicy.addResources("*");
    return secretsPolicy;
  }

  protected keepWarm(target: Function, method: string, path: string) {
    if (this.keepWarmCount % 5 == 0) {
      const part = this.keepWarmCount / 5 + 1;
      this.keepWarmRule = new Rule(
        this,
        `${this.name}-warmer-part${part}`,
        {
          description: `Scheduled request to keep Student-Api-Template ${this.name} Lambdas from shutting down`,
          ruleName: `${this.name}-warmer-part${part}`,
          schedule: Schedule.rate(Duration.minutes(10)),
        }
      );
    }

    this.keepWarmRule.addTarget(
      new LambdaFunction(target, {
        event: RuleTargetInput.fromObject(
          this.buildKeepWarmInput(method, path)
        ),
      })
    );

    this.keepWarmCount++;
  }

  protected buildKeepWarmInput(method: string, methodPath: string): any {
    return {
      path: methodPath,
      httpMethod: method.toUpperCase(),
      isBase64Encoded: false,
      headers: {
        "x-keep-warm": "true",
      },
    };
  }

  protected integrate(
    method: string,
    apiResource: IResource,
    lambdaFunc: Function,
    authorizer: CfnAuthorizer
  ) {
    const integration = new LambdaIntegration(lambdaFunc);
    apiResource.addMethod(method, integration, {
      authorizationType: AuthorizationType.COGNITO,
      authorizer: { authorizerId: authorizer.ref },
    });
  }

  protected unsecuredIntegration(
    method: string,
    apiResource: IResource,
    lambdaFunc: Function,
    keyName: string
  ) {
    const integration = new LambdaIntegration(lambdaFunc);
    const resourceMethod = apiResource.addMethod(method, integration, {
      authorizationType: AuthorizationType.NONE,
    });

    const rateLimitingKey = this.api.addApiKey(`${keyName}-key`);
    this.api.addUsagePlan(`${keyName}-usage-plan`, {
      name: keyName,
      quota: { limit: 1000, period: Period.MONTH },
      throttle: { rateLimit: 1, burstLimit: 1 }, //1 request per second limit or return 429 Too Many Requests
      apiStages: [{ api: this.api, stage: this.api.deploymentStage }],
    }).addApiKey(rateLimitingKey);
  }

  protected grantSecrets(lambda: Function) {
    lambda.addToRolePolicy(this.secretsPolicy);
  }
}

export class StudentTemplateInfrastructureStack extends StudentTemplateStack {
  constructor(scope: Construct, id: string, config: Configuration) {
    super(scope, id, `Student-Api-Template-${config.studentName}-${config.stageName}`, config);

    this.setUpDynamoLambdas();
    this.setUpRdsLambdas();
  }

  private setUpDynamoLambdas() {
    const rdsApi = this.api.root.addResource("dynamo");
    const studentApi = rdsApi.addResource("student");

    const userTable = new Table(this, "UserTable",
      {
        billingMode: BillingMode.PAY_PER_REQUEST,
        partitionKey: { name: "Id", type: AttributeType.STRING },
        pointInTimeRecovery: true,
        removalPolicy: RemovalPolicy.DESTROY,
      });



  }

  private setUpRdsLambdas() {
    const rdsApi = this.api.root.addResource("rds");
    const studentApi = rdsApi.addResource("student");



  }
}
