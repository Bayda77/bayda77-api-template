using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Xerris.DotNet.Core.Extensions;
using Student.Api.Template.Services.Model;
using Student.Api.Template.Services.Repository.DynamoDb;

namespace Student.Api.Template.Services.Services
{
    public interface IDynamoUserService
    {
        
    }

    public class DynamoUserService : IDynamoUserService
    {
        private readonly IUserDetailRequestValidator validator;
        private readonly IDynamoUserRepository repository;

        public DynamoUserService(IUserDetailRequestValidator validator, IDynamoUserRepository repository)
        {
            this.validator = validator;
            this.repository = repository;
        }


        


    }
}