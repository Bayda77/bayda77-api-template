using System;

namespace Student.Api.Template.Services.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string message) : base(message)
        {
        }
    }
}