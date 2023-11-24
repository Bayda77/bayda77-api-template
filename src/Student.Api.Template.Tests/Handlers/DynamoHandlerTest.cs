using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Moq;
using Student.Api.Template.Handlers;
using Student.Api.Template.Services.Model;
using Student.Api.Template.Services.Services;
using Student.Api.Template.Services.Repository.DynamoDb;
using Nutrien.Shared.Authorize;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;
using Xunit;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;


namespace Tests.Handlers
{
    public class DynamoHandlerTest : IDisposable
    {
        private readonly MockRepository mocks;
        private readonly Mock<IDynamoUserService> userService;
        private readonly Mock<IAuthorize> authorize;
        private readonly DynamoHandler handler;

        private const string Jwt = "jwt";

        private readonly APIGatewayProxyRequest request = new APIGatewayProxyRequest
        {
            Headers = new Dictionary<string, string> { { "Authorization", Jwt } }
        };

        public DynamoHandlerTest()
        {
            mocks = new MockRepository(MockBehavior.Strict);
            userService = mocks.Create<IDynamoUserService>();
            authorize = mocks.Create<IAuthorize>();
            handler = new DynamoHandler(userService.Object);
        }


       

        public void Dispose()
        {
            mocks.VerifyAll();
        }

    }
}
