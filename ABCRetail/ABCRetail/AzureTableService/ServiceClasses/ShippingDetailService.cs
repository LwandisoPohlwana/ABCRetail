using ABCRetail.AzureTableService.Interfaces;
using ABCRetail.Models;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ABCRetail.AzureTableService.ServiceClasses
{
    public class ShippingDetailService : IShippingDetailService
    {
        private readonly TableClient _shippingDetailTableClient;
        private readonly HttpClient _httpClient;
        private readonly string _functionUrl = "https://abcretailwebfunctions.azurewebsites.net/api/StoreShippingDetails?code=1KlW8jlZwnsQUqfCtWUeGTRkWQHR_i99TPJAteJccmHWAzFuQJUu3Q%3D%3D";

        public ShippingDetailService(TableServiceClient tableServiceClient, HttpClient httpClient)
        {
            // Initialize the TableClient for ShippingDetails
            _shippingDetailTableClient = tableServiceClient.GetTableClient("ShippingDetails");

            // Ensure that the table exists, create it if not
            _shippingDetailTableClient.CreateIfNotExists();
            _httpClient = httpClient;
        }

        public async Task AddShippingDetailAsync(ShippingDetail shippingDetail)
        {
            // Send a POST request to the Azure Function
            var json = JsonConvert.SerializeObject(shippingDetail);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_functionUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add shipping detail to Azure Function.");
            }
        }


        /*  public async Task<ShippingDetail> AddShippingDetailAsync(ShippingDetail shippingDetail)
          {
              // Set a unique RowKey for the shipping detail
              shippingDetail.RowKey = $"{shippingDetail.UserId}-{Guid.NewGuid()}";

              // Insert or update the shipping detail in Azure Table Storage
              await _shippingDetailTableClient.UpsertEntityAsync(shippingDetail);

              return shippingDetail;
          }
        */

        public List<ShippingDetail> GetShippingDetailsByUserId(string userId)
        {
            // Create a list to store the shipping details
            var shippingDetails = new List<ShippingDetail>();

            // Use the TableClient to query the entities by PartitionKey and UserId
            var queryResults = _shippingDetailTableClient.Query<ShippingDetail>(detail => detail.UserId == userId);

            // Iterate over the Pageable<ShippingDetail> synchronously and collect the results into a list
            foreach (var shippingDetail in queryResults)
            {
                shippingDetails.Add(shippingDetail);
            }

            return shippingDetails;
        }

        public async Task DeleteShippingDetailAsync(string rowKey, string userId)
        {
            // Delete the shipping detail using RowKey and UserId
            await _shippingDetailTableClient.DeleteEntityAsync("ShippingDetails", rowKey);
        }

        public async Task<List<ShippingDetail>> GetShippingDetailsByUserIdAsync(string userId)
        {
            var query = _shippingDetailTableClient.Query<ShippingDetail>(sd => sd.UserId == userId);
            List<ShippingDetail> shippingDetails = new List<ShippingDetail>();

            // Use a regular foreach loop instead of await foreach
            foreach (var detail in query)
            {
                shippingDetails.Add(detail);
            }

            return shippingDetails;
        }

    }
}
