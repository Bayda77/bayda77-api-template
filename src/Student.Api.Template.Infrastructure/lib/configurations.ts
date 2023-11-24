import {Environment} from "aws-cdk-lib";

export interface Configuration {
    readonly stageName : string,
    readonly vpcId: string;
    readonly subnetIds: string[];
    readonly securityGroupId: string;
    readonly providerArn: string;
    readonly env?: Environment;    
    readonly tags: Tag[];
    readonly studentName: string;
    readonly stackNamePrefix: string;
}

export interface Tag {
    key: string;
    value:string
}