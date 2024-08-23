using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text;
using System;

namespace FunctionApp
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([QueueTrigger("testqueue", Connection = "ConnectionString")]QueueMessage myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem.Body}");

            // Decode the Base64 message
            var messageBytes = Convert.FromBase64String(myQueueItem.Body.ToString());

            // Convert the bytes to a string
            var decodedMessage = Encoding.UTF8.GetString(messageBytes);

            // Use the decoded message
            log.LogInformation("Decoded Message: " + decodedMessage);
        }
    }
}
