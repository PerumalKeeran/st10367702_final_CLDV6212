using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ST10367702_Final_CLDV6212_Functions
{
    public class SaveContractFileFunction
    {
        private readonly string _connectionString;

        public SaveContractFileFunction(IConfiguration config)
        {
            _connectionString = config["AzureWebJobsStorage"];
        }

        [Function("SaveContractFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var share = new ShareClient(_connectionString, "contracts");
            await share.CreateIfNotExistsAsync();

            var directory = share.GetRootDirectoryClient();

            string fileName = $"contract_{Guid.NewGuid()}.txt";
            var file = directory.GetFileClient(fileName);

            string content = await req.ReadAsStringAsync();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);

            await file.CreateAsync(bytes.Length);
            await file.UploadRangeAsync(new Azure.HttpRange(0, bytes.Length), new MemoryStream(bytes));

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Contract file saved.");
            return response;
        }
    }
}
