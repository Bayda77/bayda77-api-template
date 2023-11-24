using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Student.Api.Template.Services.Net
{
    public class WebMethodException : Exception
    {
        [JsonProperty("error")]
        public WebMethodError Error { get; set; }

        public static WebMethodException Create(string source, string message)
        {
            return new WebMethodException {Error = new WebMethodError {Source = source, Message = new[] {message}}};
        }
        
        public string AllMessages=>string.Join(", ", Error?.Message ?? new List<string>());
    }

    public class WebMethodError
    {
        [JsonProperty("source")]
        public string Source { get; set; }
        
        [JsonProperty("message")]
        public IEnumerable<string> Message { get; set; }
    }
}