using System.Collections.Generic;
using Xerris.DotNet.Core.Validations;

namespace Student.Api.Template.Services.Model
{
    public interface IUserDetailRequestValidator
    {
        Validation Validate(UserRequest request);
        Validation Validate(UserRequestDetail request);
        Validation Validate(RdsUserRequestDetail request);
    }

    public class UserDetailRequestValidator : IUserDetailRequestValidator
    {
        public Validation Validate(UserRequest request) =>
            Xerris.DotNet.Core.Validations.Validate.Begin()
                .IsNotNull(request, "Invalid user Request")
                .IsNotEmpty(request.LastName, "Last Name should not be null")
                .IsNotEmpty(request.FirstName, "First Name should not be null")
                 .IsNotEmpty(request.EmailId, "Email ID should not be null")
                .Check();

        public Validation Validate(UserRequestDetail request) =>
        Xerris.DotNet.Core.Validations.Validate.Begin()
       .IsNotNull(request, "Invalid user Request")
       .IsNotEmpty(request.LastName, "Last Name should not be null")
       .IsNotEmpty(request.FirstName, "First Name should not be null")
        .IsNotEmpty(request.Email, "Email ID should not be null")
       .Check();

       public Validation Validate(RdsUserRequestDetail request) =>
        Xerris.DotNet.Core.Validations.Validate.Begin()
       .IsNotNull(request, "Invalid user Request")
       .IsNotEmpty(request.LastName, "Last Name should not be null")
       .IsNotEmpty(request.FirstName, "First Name should not be null")
        .IsNotEmpty(request.Email, "Email ID should not be null")
       .Check();
    }
}