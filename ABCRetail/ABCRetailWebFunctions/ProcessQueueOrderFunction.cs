using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ABCRetail.AzureQueueService.Interfaces;
using ABCRetail.Models;
using ABCRetail.Repositories.ServiceClasses;
using ABCRetail.AzureTableService.ServiceClasses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ABCRetail.Repositories.RepositorieInterfaces;

namespace ABCRetailWebFunctions
{
    public class ProcessQueueOrderFunction
    {
        private readonly ILogger<ProcessQueueOrderFunction> _logger;
        private readonly IOrderService _orderService;
        private readonly IQueueStorageService _queueStorageService;

        public ProcessQueueOrderFunction(ILogger<ProcessQueueOrderFunction> logger, IOrderService orderService, IQueueStorageService queueStorageService)
        {
            _logger = logger;
            _orderService = orderService;
            _queueStorageService = queueStorageService;
        }

        [Function("ProcessQueueOrder")]
        public async Task Run([Microsoft.Azure.Functions.Worker.QueueTrigger("queue-order", Connection = "AzureWebJobsStorage")] string queueMessage)
        {
            _logger.LogInformation($"Processing queue message: {queueMessage}");
            if (!string.IsNullOrEmpty(queueMessage))
            {
                try
                {
                    // Decode the Base64 message
                    var decodedMessage = Encoding.UTF8.GetString(Convert.FromBase64String(queueMessage));
                    var orderData = JsonSerializer.Deserialize<Order>(decodedMessage);

                    if (orderData != null)
                    {
                        // Get the associated order items
                        var orderItems = await _orderService.GetOrderItemsByOrderIdAsync(orderData.RowKey);

                        // Process the order by updating the inventory
                        await _orderService.UpdateInventoryAsync(orderItems);

                        _logger.LogInformation($"Order processed successfully: {orderData.RowKey}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing queue message: {ex.Message}");
                }
            }
        }
    }
}
