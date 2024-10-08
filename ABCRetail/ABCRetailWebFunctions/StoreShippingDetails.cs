using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static class StoreShippingDetails
{
    [FunctionName("StoreShippingDetails")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a request to store shipping details in Azure Table Storage.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        ShippingDetail shippingDetail = JsonConvert.DeserializeObject<ShippingDetail>(requestBody);

        if (shippingDetail == null || string.IsNullOrEmpty(shippingDetail.UserId) || string.IsNullOrEmpty(shippingDetail.AddressLine1))
        {
            return new BadRequestObjectResult("Please provide valid shipping details in the request body.");
        }

        // Connect to Azure Table Storage
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient(); // No need for TableClientConfiguration
        CloudTable table = tableClient.GetTableReference("ShippingDetails");

        // Ensure the table exists
        await table.CreateIfNotExistsAsync();

        // Insert shipping detail into table
        shippingDetail.PartitionKey = shippingDetail.UserId;
        shippingDetail.RowKey = Guid.NewGuid().ToString(); // Generate a unique RowKey for the entity
        TableOperation insertOperation = TableOperation.Insert(shippingDetail);
        await table.ExecuteAsync(insertOperation);

        return new OkObjectResult("Shipping details stored successfully.");
    }

    public class ShippingDetail : TableEntity
    {
        public string UserId { get; set; } // User associated with the shipping detail
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}
