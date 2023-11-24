using System.Threading.Tasks;
using Xerris.DotNet.Core.Aws.Secrets;
using Serilog;

namespace Student.Api.Template.Services.Shared
{
    public interface ISecretAdapter
    {
        Task<string> GetRdsReadonlyConnectionString();
        Task<string> GetRdsWriteConnectionString();
        Task<string> GetValue(string key);
    }

    public class SecretAdapter : ISecretAdapter
    {
        private readonly ISecretProvider provider;
        private readonly IApplicationConfig config;

        public SecretAdapter(ISecretProvider provider, IApplicationConfig config)
        {
            this.provider = provider;
            this.config = config;
        }

        public async Task<string> GetValue(string key)
        {
            return await provider.GetAwsSecret("Nutrien").GetSecretAsync(key).ConfigureAwait(false);
        }

        public async Task<string> GetRdsReadonlyConnectionString()
        {
            return await GetValue("ReadonlyNutrienDb").ConfigureAwait(false);
        }
        
        public async Task<string> GetRdsWriteConnectionString()
        {
            return await GetValue("ReadWriteNutrienDb").ConfigureAwait(false);
        }
    }
}