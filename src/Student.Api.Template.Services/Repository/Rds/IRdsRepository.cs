using Serilog;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xerris.DotNet.Core.Data;
using Student.Api.Template.Services.Model;


namespace Nutrien.AXP.UserManagement.Services.Repository.Rds
{
    public interface IRdsRepository
    {
        
    }

    public class RdsRepository : BaseRepository, IRdsRepository
    {
        private readonly IUnitOfWorkProvider provider;
        public RdsRepository(IPostgresConnectionBuilder connectionBuilder, IUnitOfWorkProvider provider) : base(connectionBuilder)
        {
            this.provider = provider;
        }

        

    }
}