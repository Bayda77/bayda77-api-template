using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Xerris.DotNet.Core.Extensions;
using Student.Api.Template.Services.Model;
using Nutrien.AXP.UserManagement.Services.Repository.Rds;

namespace Student.Api.Template.Services.Services
{
    public interface IRdsUserService
    {
        
    }

    public class RdsUserService : IRdsUserService
    {
        private readonly IUserDetailRequestValidator validator;
        private readonly IRdsRepository repository;

        public RdsUserService(IUserDetailRequestValidator validator, IRdsRepository repository)
        {
            this.validator = validator;
            this.repository = repository;
        }

       

    }
}