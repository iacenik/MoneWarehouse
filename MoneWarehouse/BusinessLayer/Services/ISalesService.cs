using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface ISalesService
    {
        Task<IEnumerable<Sales>> GetAllSalesAsync();
        Task<Sales> GetSaleByIdAsync(int id);
        Task<IEnumerable<Sales>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sales>> GetSalesByClientAsync(int clientId);
        Task<IEnumerable<Sales>> GetSalesByEmployeeAsync(int employeeId);
        Task<IEnumerable<Sales>> GetSalesByStatusAsync(string status);
        Task<Sales> GetSaleWithDetailsAsync(int id);
        Task CreateSaleAsync(Sales sale); // Stok kontrolü yapacak ve satış detaylarına göre stokları düşecek
        Task UpdateSaleAsync(Sales sale); // Stok değişimlerini yönetecek
        Task DeleteSaleAsync(int id); // Satış silindiğinde stokları geri ekleyecek
        Task<decimal> CalculateTotalSalesAmountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetMonthlySalesReportAsync(int year);
        Task<Dictionary<int, decimal>> GetSalesByClientReportAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, decimal>> GetSalesByEmployeeReportAsync(DateTime startDate, DateTime endDate);
    }
}
