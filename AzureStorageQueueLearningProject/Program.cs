
using Azure.Storage.Queues;
using System.Text;

namespace AzureStorageQueueLearningProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var azureStorageQueue = new AzureStorageQueue();

            for (int i = 0; i < 2; i++)
            {
                await azureStorageQueue.SendMessageAsyc($"Test Message {i}");
            }

            //await azureStorageQueue.PeekMessagesAsync(10);

            await azureStorageQueue.GetQueueLength();

            //await azureStorageQueue.ReceveieMessageAsync();

            await azureStorageQueue.GetQueueLength();
        }
    }

    public class AzureStorageQueue
    {
        private string _queueName;
        private string _connectionString;
        private QueueClient _client;

        public AzureStorageQueue() 
        {
            _queueName = "testqueue";
            _connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

            // If you’re dealing with binary data or non-UTF8 encoded text, Base64 encoding can be used to ensure that the data is correctly represented and transferred.
            // For instance, if you’re sending files or binary attachments through Azure Queues, you would Base64-encode the content to handle it as a text message.
            // encoded message text stored in Queue Table 
            var queueClientOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            _client = new QueueClient(_connectionString, _queueName, queueClientOptions);

            // string message text stored in Queue Table 
            //_client = new QueueClient(_connectionString, _queueName);
        }

        public async Task SendMessageAsyc(string message)
        {
            var messageByte = Encoding.UTF8.GetBytes(message);
            await _client.SendMessageAsync(Convert.ToBase64String(messageByte));

            Console.WriteLine($"Message sent - {message}");
        }

        public async Task PeekMessagesAsync(int maxMessageCount)
        {
            var messages = await _client.PeekMessagesAsync(maxMessageCount);

            foreach (var message in messages.Value)
            {
                Console.WriteLine($"Message Id: {message.MessageId}");
                Console.WriteLine($"Message Body: {message.Body}");


                // Decode the Base64 message
                var messageBytes = Convert.FromBase64String(message.Body.ToString());

                // Convert the bytes to a string
                var decodedMessage = Encoding.UTF8.GetString(messageBytes);

                // Use the decoded message
                Console.WriteLine("Decoded Message: " + decodedMessage);
            }
        }

        public async Task ReceveieMessageAsync()
        { 
            var messages = await _client.ReceiveMessagesAsync(10);

            foreach (var message in messages.Value)
            {
                Console.WriteLine($"Receive Message Id: {message.MessageId}");
                Console.WriteLine($"Receive Message Body: {message.Body}");


                // Decode the Base64 message
                var messageBytes = Convert.FromBase64String(message.Body.ToString());

                // Convert the bytes to a string
                var decodedMessage = Encoding.UTF8.GetString(messageBytes);

                // Use the decoded message
                Console.WriteLine("Receive Decoded Message: " + decodedMessage);

                // delete. in azure function queue trigger this is already done by the azure function, no need to explicitly delete it.
                await _client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }

        public async Task GetQueueLength()
        {
            var count = (await _client.GetPropertiesAsync()).Value.ApproximateMessagesCount;
            Console.WriteLine($"ApproximateMessagesCount: {count}");
        }
    }
}
