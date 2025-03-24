using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IPlıntusDailyService
    {
        Task<IEnumerable<PlıntusDaily>> GetAllEntriesAsync();
        Task<PlıntusDaily> GetEntryByIdAsync(int id);
        Task<IEnumerable<PlıntusDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PlıntusDaily>> GetEntriesByEmployeeAsync(int employeeId);
        Task<IEnumerable<PlıntusDaily>> GetEntriesByStockIdAsync(int stockId);
        Task CreateEntryAsync(PlıntusDaily entry); // Bu metod stok miktarını artıracak
        Task UpdateEntryAsync(PlıntusDaily entry, int oldQuantity); // Miktar değişikliğini stoka yansıtacak
        Task DeleteEntryAsync(int id); // Stok miktarını azaltacak
        Task<int> GetTotalProductionByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, int>> GetProductionByEmployeeAsync(DateTime startDate, DateTime endDate);
    }

}
