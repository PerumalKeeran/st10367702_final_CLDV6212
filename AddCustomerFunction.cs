using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ST10367702_Final_CLDV6212_Functions
{
    public class AddCustomerFunction
    {
        private readonly string _connectionString;

        public AddCustomerFunction(IConfiguration config)
        {
            _connectionString = config["AzureWebJobsStorage"];
        }

        [Function("AddCustomer")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var tableClient = new TableServiceClient(_connectionString)
                                .GetTableClient("Customers");

            await tableClient.CreateIfNotExistsAsync();

            var data = await req.ReadFromJsonAsync<CustomerEntity>();

            data.PartitionKey = "Customer";
            data.RowKey = Guid.NewGuid().ToString();

            await tableClient.AddEntityAsync(data);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Customer added successfully.");
            return response;
        }
    }

    public class CustomerEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
