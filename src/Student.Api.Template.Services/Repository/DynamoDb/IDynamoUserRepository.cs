using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Serilog;
using Xerris.DotNet.Core.Aws.Repositories.DynamoDb;
using Student.Api.Template.Services.Model;
using System.Collections.Generic;
using System;

namespace Student.Api.Template.Services.Repository.DynamoDb
{
    public interface IDynamoUserRepository
    {
    
    
    }

    public class DynamoUserRepository : BaseRepository<UserRequestDetail>, IDynamoUserRepository
    {
        public DynamoUserRepository(IApplicationConfig config, IAmazonDynamoDB dbClient) :
            base(dbClient, Environment.GetEnvironmentVariable("USER_TABLE_NAME"))
        {
        }



    }
}
