using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface ISalesDetailService
    {
        Task<IEnumerable<SalesDetail>> GetAllDetailsAsync();
        Task<SalesDetail> GetDetailByIdAsync(int id);
        Task<IEnumerable<SalesDetail>> GetDetailsBySalesIdAsync(int salesId);
        Task<IEnumerable<SalesDetail>> GetDetailsByProductTypeAsync(string productType);
        Task<IEnumerable<SalesDetail>> GetDetailsByStockIdAsync(int stockId, string productType);
        Task CreateDetailAsync(SalesDetail detail); // Stok miktarını düşürecek
        Task UpdateDetailAsync(SalesDetail detail, int oldQuantity, int? oldProductId, string oldProductType); // Stok değişikliğini yönetecek
        Task DeleteDetailAsync(int id); // Stok miktarını geri ekleyecek
        Task<Dictionary<string, int>> GetProductSalesCountAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetProductSalesAmountAsync(DateTime startDate, DateTime endDate);
    }
}
