using System;
using Amazon.DynamoDBv2.DataModel;
using Xerris.DotNet.Core.Time;

namespace Student.Api.Template.Services.Model
{    
    public class RdsUserRequestDetail
    {
        public RdsUserRequestDetail()
        {
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}