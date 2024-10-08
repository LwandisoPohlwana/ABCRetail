using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace ABCRetailWebFunctions
{
    public class UploadFileFunction
    {
        private readonly ILogger<UploadFileFunction> _logger;
        private readonly string _connectionString; // Connection string for Blob Storage

        public UploadFileFunction(ILogger<UploadFileFunction> logger)
        {
            _logger = logger;
            _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage"); // Set your Blob Storage connection string
        }

        [FunctionName("UploadFile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadfile")] HttpRequest req)
        {
            _logger.LogInformation("Processing file upload");

            // Retrieve the file from the request
            var file = req.Form.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("Please upload a file.");
            }

            var fileName = file.FileName;

            try
            {
                // Create a BlobServiceClient
                var blobServiceClient = new BlobServiceClient(_connectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("productimages");

                // Create the container if it doesn't exist
                await blobContainerClient.CreateIfNotExistsAsync();

                // Upload the file to Blob Storage
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using (var fileStream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(fileStream, true); // Overwrite if the file already exists
                }

                var blobUrl = blobClient.Uri.ToString(); // Get the URL of the uploaded file
                _logger.LogInformation("File uploaded successfully. Blob URL: {BlobUrl}", blobUrl);

                return new OkObjectResult(new { Url = blobUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file to Blob Storage: {ex}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
