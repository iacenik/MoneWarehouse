using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IPlıntusStockService
    {
        Task<IEnumerable<PlıntusStock>> GetAllStocksAsync();
        Task<PlıntusStock> GetStockByIdAsync(int id);
        Task<IEnumerable<PlıntusStock>> GetStocksByCodeAsync(string code);
        Task<PlıntusStock> GetStockWithDailyEntriesAsync(int id);
        Task CreateStockAsync(PlıntusStock stock);
        Task UpdateStockAsync(PlıntusStock stock);
        Task DeleteStockAsync(int id);
        Task IncreaseStockQuantityAsync(int stockId, int quantity); // Stok artırma
        Task DecreaseStockQuantityAsync(int stockId, int quantity); // Stok azaltma
        Task<int> GetTotalStockQuantityAsync(); // Toplam stok miktarı
        Task<List<PlıntusStock>> GetLowStockItemsAsync(int threshold); // Kritik stok seviyesindeki ürünler
    }
}
