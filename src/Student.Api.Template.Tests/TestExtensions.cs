using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;

namespace Tests
{
    public static class TestExtensions
    {
        public static APIGatewayProxyRequest AddQueryParams(this APIGatewayProxyRequest request, params KeyValuePair<string, string>[] queryItems)
        {
            request.QueryStringParameters = new Dictionary<string, string>();
            foreach (var (key, value) in queryItems) 
                request.QueryStringParameters.Add(key, value);
            return request;
        }
    }
}