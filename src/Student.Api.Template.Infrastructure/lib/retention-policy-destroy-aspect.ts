import { CfnResource, IAspect, RemovalPolicy } from "aws-cdk-lib";
import { IConstruct } from "constructs";

// Set Retention Policy of all resources to DESTROY
export class RetentionPolicyDestroyAsepct implements IAspect {
    visit(node: IConstruct): void {
        if(CfnResource.isCfnResource(node)) {
            node.applyRemovalPolicy(RemovalPolicy.DESTROY);
        }
    } 
}