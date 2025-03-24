using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IInjectionDailyService
    {
        Task<IEnumerable<InjectionDaily>> GetAllEntriesAsync();
        Task<InjectionDaily> GetEntryByIdAsync(int id);
        Task<IEnumerable<InjectionDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InjectionDaily>> GetEntriesByEmployeeAsync(int employeeId);
        Task<IEnumerable<InjectionDaily>> GetEntriesByStockIdAsync(int stockId);
        Task CreateEntryAsync(InjectionDaily entry); // Bu metod stok miktarını artıracak
        Task UpdateEntryAsync(InjectionDaily entry, int oldQuantity); // Miktar değişikliğini stoka yansıtacak
        Task DeleteEntryAsync(int id); // Stok miktarını azaltacak
    }
}
