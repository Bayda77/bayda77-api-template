using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Serilog;
using Xerris.DotNet.Core.Aws.Sqs;
using Xerris.DotNet.Core.Extensions;

namespace Student.Api.Template.Services.Publishers
{
    public class BaseSqsPublisher<T> : IPublishSqsMessages<T> where T : class
    {
        private readonly string queueUrl;
        private readonly AmazonSQSClient client;

        protected BaseSqsPublisher(string queueUrl)
        {
            this.queueUrl = queueUrl;
            client = new AmazonSQSClient();
        }

        public async Task<bool> SendMessageAsync(T message, Dictionary<string, MessageAttributeValue> attributes)=>
            await Publish(message, attributes).ConfigureAwait(false);

        public async Task<bool> SendMessageAsync(T message)=>await Publish(message);
        
        public async Task<bool> SendMessageAsync(T message, int delaySeconds)=>await Publish(message, null, delaySeconds);

        private async Task<bool> Publish(T message, Dictionary<string, MessageAttributeValue> attributes = null, int? delaySeconds = null)
        {
            var body = message.ToJson();
            var request = new SendMessageRequest(queueUrl, body);
            
            if (attributes != null)
                request.MessageAttributes = attributes;

            if (delaySeconds.HasValue)
                request.DelaySeconds = delaySeconds.Value;

            var response = await client.SendMessageAsync(request).ConfigureAwait(false);

            var successful = response.HttpStatusCode == HttpStatusCode.OK;

            if (!successful)
                Log.Information("Unable to send message to queue: '{message}'", body);

            return successful;
        }

        public async Task<bool> SendMessagesAsync(IEnumerable<T> messages)
        {
            var batch = messages.Select(x=>new SendMessageBatchRequestEntry(Guid.NewGuid().ToString(), x.ToJson())).ToList();
            if (!batch.Any())
            {
                Log.Information("Attempting to send empty batch to queue: '{queueUrl}'", queueUrl);
                return true;
            }

            var request = new SendMessageBatchRequest(queueUrl, batch);
            var response = await client.SendMessageBatchAsync(request).ConfigureAwait(false);

            if (response.HttpStatusCode != HttpStatusCode.OK)
                Log.Information("Unable to send message to queue: '{message}'", batch.ToJson());

            return response.HttpStatusCode == HttpStatusCode.OK && !response.Failed.Any();
        }
    }
}