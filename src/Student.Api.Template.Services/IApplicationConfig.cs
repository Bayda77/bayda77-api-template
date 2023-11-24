using Amazon.Extensions.NETCore.Setup;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.Secrets;

namespace Student.Api.Template.Services
{
    public interface IApplicationConfig
    {
        SecretConfigCollection SecretConfigurations { get; }
        string UserTableName { get; set; }
        string IdentityHeader { get; }
        string WebMethodsIdentityHeader { get; }
        string NutrienWebMethodsBaseAddress { get; }
    }

    public class ApplicationConfig : IApplicationConfig, IApplicationConfigBase
    {

        public SecretConfigCollection SecretConfigurations { get; set; }
        public AWSOptions AwsOptions { get; set; }
        public string UserTableName { get; set; }
        public string IdentityHeader { get; set; }
        public string WebMethodsIdentityHeader { get; set; }
        public string NutrienWebMethodsBaseAddress { get; set; }
    }

}