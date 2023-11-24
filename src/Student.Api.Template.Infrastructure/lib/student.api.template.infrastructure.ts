#!/usr/bin/env node
import "source-map-support/register";
import { Configuration } from "./configurations";
import { StudentTemplateInfrastructureStack } from "./student.api.template.infrastructure-stack";
import { App, Aspects, Tags } from "aws-cdk-lib";
import { RetentionPolicyDestroyAsepct } from "./retention-policy-destroy-aspect";

const commonTags = [
  { key: "department-functionalarea", value: "sales and tdl-sales tdl and ibp" },
  { key: "pipeline", value: "student-api-template" },
  { key: "application", value: "Hello Nutrien" }];

const configurations: { [key: string]: Configuration } = {
  ["dev"]: {
    stageName: "dev",
    studentName: "ENTER-NAME",
    vpcId: "vpc-0152f6922ab317ffe",
    subnetIds: ["subnet-0e31abdeef8f280db", "subnet-065eb705639118766"],
    providerArn: "arn:aws:cognito-idp:us-east-2:840375332198:userpool/us-east-2_QNW2FlS0K",
    securityGroupId: "sg-0c7fd579e099199ed",
    env: {
      account: process.env.CDK_DEFAULT_ACCOUNT,
      region: "us-east-2",
    },
    tags: [
      { key: "environment", value: "dev" },
      ...commonTags,
    ],
    stackNamePrefix: "studentApi",
  },

  ["stage"]: {
    stageName: "stage",
    studentName: "ENTER-NAME",
    vpcId: "vpc-0152f6922ab317ffe",
    subnetIds: ["subnet-0e31abdeef8f280db", "subnet-065eb705639118766"],
    providerArn: "arn:aws:cognito-idp:us-east-2:840375332198:userpool/us-east-2_jEIys39Ub",
    securityGroupId: "sg-0c7fd579e099199ed",
    env: {
      account: process.env.CDK_DEFAULT_ACCOUNT,
      region: "us-east-2",
    },
    tags: [
      { key: "environment", value: "stage" },
      ...commonTags,
    ],
    stackNamePrefix: "studentApi",
  },
  ["prod"]: {
    stageName: "prod",
    studentName: "",
    vpcId: "vpc-00c777e36a1f4ee38",
    subnetIds: ["subnet-0f5ae3507875c3d2f", "subnet-0db711871a6a4dfff"],
    providerArn: "arn:aws:cognito-idp:ap-southeast-2:475979636662:userpool/ap-southeast-2_O4eGfnzaQ",
    securityGroupId: "sg-02b064bf1f623fe16",
    env: {
      account: process.env.CDK_DEFAULT_ACCOUNT,
      region: process.env.CDK_DEFAULT_REGION,
    },
    tags: [
      { key: "environment", value: "prod" },
      ...commonTags,
    ],
    stackNamePrefix: "studentApi",
  },
};

const app = new App();

const buildEnvironment = (app.node.tryGetContext("env") || "dev").trim().toLowerCase();

const config = configurations[buildEnvironment];

new StudentTemplateInfrastructureStack(app, `Student-Api-Template-${config.studentName}-${config.stageName}`, config);

Aspects.of(app).add(new RetentionPolicyDestroyAsepct());

config.tags.forEach(tag => Tags.of(app).add(tag.key, tag.value));