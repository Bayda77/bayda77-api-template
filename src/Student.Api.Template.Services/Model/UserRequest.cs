using System;
using Newtonsoft.Json;

namespace Student.Api.Template.Services.Model
{
    public class UserRequest
    {
        [JsonIgnore] public readonly string Id = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }   
    }
}