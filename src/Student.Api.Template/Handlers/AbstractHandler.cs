using Amazon.Lambda.APIGatewayEvents;
using Student.Api.Template.Services.Model;
using Nutrien.Shared.Authorize;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Aws.Lambdas;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Student.Api.Template.Handlers
{
      public abstract class AbstractHandler : BaseHandler
    {
    }
    public abstract class AbstractHandler<T> : BaseHandler where T : BaseResponse, new()
    {
        //private readonly IAuthorize authorize;
        
        protected AbstractHandler()
        {
            //this.authorize = authorize;            
        }

        // protected async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest request,
        //     Func<IAuthorizeResult, Task<APIGatewayProxyResponse>> func,
        //     [CallerMemberName] string caller = null)
        // {
        //     try
        //     {
        //         Log.Information($"request: {request.Body.ToJson()}");
        //         Log.Information($"path: {request.Path}");
        //         var authorizeResult = authorize.Authorize(GetIdentity(request));
        //         return authorizeResult.IsAuthorized
        //             ? await func(authorizeResult)
        //             : caller.UnAuthorized();
        //     }
        //     catch (ValidationException e)
        //     {
        //         Log.Error(e, "Validation error in {caller}", caller);
        //         return new T {Success = false, ErrorMessage = e.Message}.BadRequest();
        //     }
        //     catch (SecurityException e)
        //     {
        //         Log.Error(e, "Access denied to {caller}", caller);
        //         return e.Message.UnAuthorized();
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e, "Unexpected error encountered in {caller}", caller);
        //         return new T {Success = false, ErrorMessage = e.Message}.Error();
        //     }
        // }
        
        protected async Task<APIGatewayProxyResponse> ExecuteUnsecuredAsync(
            Func<Task<APIGatewayProxyResponse>> func, [CallerMemberName] string caller = null)
        {
            try
            {
                return await func().ConfigureAwait(false);
            }
            catch (ValidationException e)
            {
                Log.Error(e, "Validation error in {caller}", caller);
                return new MessageResponse {Success = false, ErrorMessage = e.Message}.BadRequest();
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e, "InvalidOperationException in {caller}: {message}", caller, e.Message);
                return new MessageResponse {Success = false, ErrorMessage = e.Message}.NotFound();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unexpected error encountered in {caller}", caller);
                return new MessageResponse {Success = false, ErrorMessage = e.Message}.Error();
            }
        }
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };        
    }
}
