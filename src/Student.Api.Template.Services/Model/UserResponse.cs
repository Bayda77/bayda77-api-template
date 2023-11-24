using System.Collections.Generic;

namespace Student.Api.Template.Services.Model
{
    public class UserResponse : BaseResponse
    {
        public UserRequestDetail User { get; set; }
        

        public RdsUserRequestDetail RdsUser { get; set; }
        public IEnumerable<RdsUserRequestDetail> RdsUsers { get; set; }
    }    
}