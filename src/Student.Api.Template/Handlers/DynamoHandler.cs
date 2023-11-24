using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Student.Api.Template.Services.Model;
using Student.Api.Template.Services.Services;
using Serilog;
using System.Threading.Tasks;
using Xerris.DotNet.Core;
using Xerris.DotNet.Core.Aws.Api;

namespace Student.Api.Template.Handlers
{
    public class DynamoHandler : AbstractHandler<UserResponse>
    {
        private readonly IDynamoUserService userService;

        public DynamoHandler() : this(IoC.Resolve<IDynamoUserService>())
        {
        }
        public DynamoHandler(IDynamoUserService userService) : base()
        {
            this.userService = userService;
        }


        
   }
}