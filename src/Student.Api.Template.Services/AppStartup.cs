using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrien.Shared;
using Serilog;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.IoC;
using Xerris.DotNet.Core.Cache;

namespace Student.Api.Template.Services
{
    public sealed class AppStartup : IAppStartup
    {
        public IConfiguration StartUp(IServiceCollection collection)
        {
            var builder = new ApplicationConfigurationBuilder<ApplicationConfig>();
            var appConfig = builder.Build();

            collection.AddSingleton<IApplicationConfig>(appConfig);
            collection.AddDefaultAWSOptions(appConfig.AwsOptions);            
            collection.AddSecretProvider(appConfig.SecretConfigurations);
            collection.AddAWSService<IAmazonDynamoDB>();
            collection.AddSingleton<ICache>(new WaitToFinishMemoryCache(2, 10));
            //collection.AddJwtAuthorize(appConfig.CognitoPoolId, appConfig.CognitoPublicWebKey);
            collection.AutoRegister(GetType().Assembly);

            return builder.Configuration;
        }

        public void InitializeLogging(IConfiguration configuration, Action<IConfiguration> defaultConfig)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}