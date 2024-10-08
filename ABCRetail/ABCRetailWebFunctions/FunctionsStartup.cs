using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using ABCRetail.AzureQueueService.Interfaces;
using ABCRetail.Repositories.ServiceClasses;
using ABCRetail.Repositories.RepositorieInterfaces;
using ABCRetail.AzureQueueService.Service;

[assembly: FunctionsStartup(typeof(ABCRetailWebFunctions.Startup))] // Use "Startup" instead of "FunctionsStartup" to avoid confusion

namespace ABCRetailWebFunctions
{
    public class Startup : FunctionsStartup // Inherit from FunctionsStartup correctly
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register services for dependency injection here
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IQueueStorageService, QueueStorageService>();
        }
    }
}
