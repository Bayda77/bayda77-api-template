using System.Threading.Tasks;
using Serilog;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Net;

namespace Student.Api.Template.Services.Publishers
{
    public interface IPublishSnsMessages
    {
        Task<bool> SendMessageAsync(string message, string subject);        
    }    
    public class BaseSnsPublisher : IPublishSnsMessages
    {
        private readonly string snsTopicArn;
        private readonly AmazonSimpleNotificationServiceClient client;

        protected BaseSnsPublisher(string snsTopicArn)
        {
            this.snsTopicArn = snsTopicArn;
            client = new AmazonSimpleNotificationServiceClient();
        }

        public async Task<bool> SendMessageAsync(string message, string subject)
        {            
            var request = new PublishRequest(){
                    Message = message,
                    TopicArn = snsTopicArn,
                    Subject = subject
                };  
                
            Log.Information($"Publishing message {message} to sns topic {snsTopicArn}");

            var response = await client.PublishAsync(request).ConfigureAwait(false);            

            var successful = response.HttpStatusCode == HttpStatusCode.OK;

            if (!successful)
                Log.Information($"Unable to send message to sns topic: {snsTopicArn}"); 

            return successful;          
        }      
    }
}