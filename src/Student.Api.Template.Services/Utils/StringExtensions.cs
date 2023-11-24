using System.Web;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Core.Validations;

namespace Student.Api.Template.Services.Utils
{
    public static class StringExtensions
    {
        public static string AddToQueryString(this string uri, string key, string value)
        {
            if (Validate.Begin().IsNotEmpty(key, "key")
                                .IsNotEmpty(value, "value)").IsValid())
            {
                return uri + (uri.IsNullOrEmpty() ? "?" : "&") + $"{key}={value.UrlEncoded()}";
            }
            return uri;
        }
        
        public static string AddAllowableEmptyValueToQueryString(this string uri, string key, string value)
        {
            if (Validate.Begin().IsNotEmpty(key, "key").IsValid())
            {
                return uri + (uri.IsNullOrEmpty() ? "?" : "&") + $"{key}={value.UrlEncoded()}";
            }
            return uri;
        }

        public static string UrlEncoded(this string value)
        {
            return value.IsNullOrEmpty() ? value : HttpUtility.UrlEncode(value);
        }
    }
}