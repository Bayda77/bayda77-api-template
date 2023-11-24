using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Student.Api.Template.Services.Shared;
using Serilog;

namespace Nutrien.AXP.UserManagement.Services.Repository.Rds
{
    public class PostgresConnectionBuilder : IPostgresConnectionBuilder
    {
        private readonly ISecretAdapter adapter;

        public PostgresConnectionBuilder(ISecretAdapter adapter)
        {
            this.adapter = adapter;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connectionString = await adapter.GetRdsWriteConnectionString().ConfigureAwait(false);            
            return CreateConnection(connectionString);
        }

        public async Task<IDbConnection> CreateReadConnectionAsync()
        {
            var readonlyConnectionString = await adapter.GetRdsReadonlyConnectionString().ConfigureAwait(false);
            return CreateConnection(readonlyConnectionString);
        }

        private static NpgsqlConnection CreateConnection(string connString)
        {
            return new NpgsqlConnection(connString);
        }
    }
}