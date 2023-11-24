using System;
using Amazon.DynamoDBv2.DataModel;
using Xerris.DotNet.Core.Time;

namespace Student.Api.Template.Services.Model
{
    //[DynamoDBTable("StudentDetails")]
    public class UserRequestDetail
    {
        public UserRequestDetail()
        {
        }

        public UserRequestDetail(UserRequest request)
        {
            Id = request.Id;
            Email = request.EmailId.ToLower();
            FirstName = request.FirstName;           
            LastName = request.LastName;           
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}