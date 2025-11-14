using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ST10367702_Final_CLDV6212_Functions
{
    public class SendOrderToQueueFunction
    {
        private readonly string _connectionString;

        public SendOrderToQueueFunction(IConfiguration config)
        {
            _connectionString = config["AzureWebJobsStorage"];
        }

        [Function("SendOrderToQueue")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var queue = new QueueClient(_connectionString, "orderqueue");
            await queue.CreateIfNotExistsAsync();

            string message = await req.ReadAsStringAsync();
            await queue.SendMessageAsync(message);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Order queued.");
            return response;
        }
    }
}
