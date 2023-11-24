using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Serilog;
using Xerris.DotNet.Core.Validations;

namespace Student.Api.Template.Services.Utils
{
    public static class SqsMessageMessageExtension
    {
        public static async Task ExecuteMessage(this SQSEvent.SQSMessage message, Func<SQSEvent.SQSMessage, Task> action, [CallerMemberName] string path = null)
        {
            try
            {
                Log.Information("SQSMessage.Body {body}", message.Body);
                await action(message).ConfigureAwait(false);
            }
            catch (ValidationException e)
            {
                Log.Error(e, "SQS message request is invalid: {message}", e.Message);
                Log.Error(message.Body);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to process message");
                Log.Error(message.Body);
                throw;
            }
        }
    }
}