using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xerris.DotNet.Core.Extensions;

namespace Student.Api.Template.Services.Utils
{
    public static class HttpRequestMessageExtensions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        public static HttpRequestMessage AddJsonContent<T>(this HttpRequestMessage message, T content) where T : class
        {
            var json = content.ToJson(Settings);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return message;
        }
        
        public static HttpRequestMessage AddHeader(this HttpRequestMessage message, string key, string value)
        {
            message.Headers.Add(key, value);
            return message;
        }
    }
}