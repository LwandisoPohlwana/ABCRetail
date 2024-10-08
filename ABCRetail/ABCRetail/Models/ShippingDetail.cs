using Azure.Data.Tables;
using Azure;

namespace ABCRetail.Models
{
    public class ShippingDetail : ITableEntity
    {
        public string PartitionKey { get; set; } = "ShippingDetails"; // Common partition key for all shipping details
        public string RowKey { get; set; } // Unique identifier for each shipping detail
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string UserId { get; set; } // User associated with the shipping detail
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

}
