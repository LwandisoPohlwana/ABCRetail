using ABCRetail.Models;

namespace ABCRetail.AzureTableService.Interfaces
{
    public interface IShippingDetailService
    {
        Task AddShippingDetailAsync(ShippingDetail shippingDetail);
        List<ShippingDetail> GetShippingDetailsByUserId(string userId);
        Task DeleteShippingDetailAsync(string rowKey, string userId);
        Task<List<ShippingDetail>> GetShippingDetailsByUserIdAsync(string userId);
    }

}
