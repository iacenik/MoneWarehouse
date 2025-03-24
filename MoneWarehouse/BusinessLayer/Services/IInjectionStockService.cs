using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IInjectionStockService
    {
        Task<IEnumerable<InjectionStock>> GetAllStocksAsync();
        Task<InjectionStock> GetStockByIdAsync(int id);
        Task<IEnumerable<InjectionStock>> GetStocksByCodeAsync(string code);
        Task<InjectionStock> GetStockWithDailyEntriesAsync(int id);
        Task CreateStockAsync(InjectionStock stock);
        Task UpdateStockAsync(InjectionStock stock);
        Task DeleteStockAsync(int id);
        Task IncreaseStockQuantityAsync(int stockId, int quantity); // Stok artırma
        Task DecreaseStockQuantityAsync(int stockId, int quantity); // Stok azaltma
        Task<int> GetTotalStockQuantityAsync(); // Toplam stok miktarı
        Task<List<InjectionStock>> GetLowStockItemsAsync(int threshold); // Kritik stok seviyesindeki ürünler
    }

}
