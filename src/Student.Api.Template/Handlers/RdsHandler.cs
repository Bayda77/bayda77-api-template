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
    public class RdsHandler : AbstractHandler<UserResponse>
    {
        private readonly IRdsUserService userService;

        public RdsHandler() : this(IoC.Resolve<IRdsUserService>())
        {
        }

        public RdsHandler(IRdsUserService userService) : base()
        {
            this.userService = userService;
        }

        

    }
}