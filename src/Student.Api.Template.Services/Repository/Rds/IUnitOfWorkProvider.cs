using System.Threading.Tasks;
using Serilog;

namespace Nutrien.AXP.UserManagement.Services.Repository.Rds
{
    public interface IUnitOfWorkProvider
    {
        Task<IUnitOfWork> CreateAsync();
        Task<IUnitOfWork> CreateReadonlyAsync();
    }
    
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        private readonly IPostgresConnectionBuilder builder;

        public UnitOfWorkProvider(IPostgresConnectionBuilder builder)
        {
            this.builder = builder;
        }
        
        public async Task<IUnitOfWork> CreateAsync()
        {
            Log.Debug("Provider is about to request a new connection");
            var connection = await builder.CreateConnectionAsync().ConfigureAwait(false);
            connection.Open();
            Log.Debug("New connection opened.");
            return new UnitOfWork(connection);
        }

        public async Task<IUnitOfWork> CreateReadonlyAsync()
        {
            Log.Debug("Provider is about to request a new connection");
            var connection = await builder.CreateConnectionAsync().ConfigureAwait(false);
            connection.Open();
            return new ReadonlyUnitOfWork(connection);
        }
    }
}