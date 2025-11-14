using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ST10367702_Final_CLDV6212_Functions
{
    public class UploadProductImageFunction
    {
        private readonly string _connectionString;

        public UploadProductImageFunction(IConfiguration config)
        {
            _connectionString = config["AzureWebJobsStorage"];
        }

        [Function("UploadProductImage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var container = new BlobContainerClient(_connectionString, "productimages");
            await container.CreateIfNotExistsAsync();

            var file = req.Body;
            string fileName = $"product_{Guid.NewGuid()}.jpg";

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(file);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Image uploaded successfully.");
            return response;
        }
    }
}
