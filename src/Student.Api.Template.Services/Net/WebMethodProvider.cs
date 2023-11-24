using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Student.Api.Template.Services.Utils;
using Serilog;
using Xerris.DotNet.Core.Aws.Api;
using Xerris.DotNet.Core.Aws.Secrets;
using Xerris.DotNet.Core.Extensions;

namespace Student.Api.Template.Services.Net
{
    public interface IWebMethodProvider
    {
        HttpRequestMessage AddIdentityTo(HttpRequestMessage message, string identity);
        Task<T> SendAsync<T>(HttpRequestMessage requestMessage, Func<string, T> serializer, string secretKey, [CallerMemberName] string memberName = null);
    }

    public class WebMethodProvider : IDisposable, IWebMethodProvider
    {
        private static HttpClient client;
        private ISecretProvider Provider { get; }
        private IApplicationConfig Config { get; }

        public WebMethodProvider(ISecretProvider provider, IApplicationConfig config)
        {
            Provider = provider;
            Config = config;
        }

        private HttpClient Client
        {
            get
            {
                lock (this)
                {
                    if (client == null) Init();
                    return client;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Init()
        {
            if (client != null) return;

            client = new HttpClient {BaseAddress = new Uri(Config.NutrienWebMethodsBaseAddress)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<string> GetAuthenticationCredentials(string secretKey)
        {
            var secret = Provider.GetAwsSecret("Nutrien");
            var apiKey = await secret.GetSecretAsync(secretKey).ConfigureAwait(false);
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey));
        }

        public HttpRequestMessage AddIdentityTo(HttpRequestMessage message, string identity)=>
            message.AddHeader(Config.WebMethodsIdentityHeader, identity)
                .AddHeader("Accept", "application/json");

        public async Task<T> SendAsync<T>(HttpRequestMessage requestMessage, Func<string, T> serializer, string secretKey,
            [CallerMemberName] string memberName = null)
        {
            try
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Basic", await GetAuthenticationCredentials(secretKey));
                Log.Information("Web-method request:  '{requestMessage}'", requestMessage);
                Log.Information("Web-method payload: '{requestMessage}'",
                    requestMessage.Content?.ReadAsStringAsync().Result.ToJson() ?? "No content");

                using var response = await Client.SendAsync(requestMessage).ConfigureAwait(false);
                Log.Information("Response Status Code from web-methods:  '{statusCode}'", (int) response.StatusCode);

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Log.Information("String response from web-methods: '{responseString}'", responseString);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw WebMethodException.Create("WebMethodResponse", "Unauthorized");

                if (!response.IsSuccessStatusCode)
                {
                    throw responseString.FromJson<WebMethodException>() ??
                          WebMethodException.Create("WebMethodResponse", $"error: {response.StatusCode}");
                }

                var failedResult = TrParseErrorResponse(responseString);
                if (failedResult?.Result?.Success ?? true)
                    return serializer(responseString);

                throw new WebMethodException {Error = new WebMethodError {Message = failedResult.Result?.Message}};

            }
            catch (Exception e)
            {
                Log.Error("HttpRequestMessage: '{requestMessage}'", requestMessage.ToJson());
                Log.Error("Content: '{requestMessage}'",
                    requestMessage?.Content?.ReadAsStringAsync().Result.ToJson() ?? "No content");
                Log.Error("BaseAddress: {BaseAddress}", Client.BaseAddress);
                Log.Error("Request Uri: {RequestUri}", requestMessage?.RequestUri);
                Log.Error(e, "Error calling {memberName}", memberName);
                throw;
            }
        }

        private static FailedResponse TrParseErrorResponse(string responseString)
        {
            try
            {
                return responseString.FromJson<FailedResponse>();
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            client?.Dispose();
            client = null;
        }

        private class FailedResponse
        {
            [JsonProperty("result")] 
            public FailedResult Result { get; set; }
        }

        private class FailedResult
        {
            [JsonProperty("success")] 
            public bool Success { get; set; } = true;
            [JsonProperty("message")] 
            public IEnumerable<string> Message { get; set; }
        }
    }
}