using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class PlıntusStockService : IPlıntusStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlıntusStockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlıntusStock>> GetAllStocksAsync()
        {
            return await _unitOfWork.PlintusStocks.GetAllAsync();
        }

        public async Task<PlıntusStock> GetStockByIdAsync(int id)
        {
            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(id);
            if (stock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            return stock;
        }

        public async Task<IEnumerable<PlıntusStock>> GetStocksByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Kod bilgisi boş olamaz.");

            return await _unitOfWork.PlintusStocks.GetStocksByCodeAsync(code);
        }

        public async Task<PlıntusStock> GetStockWithDailyEntriesAsync(int id)
        {
            var stock = await _unitOfWork.PlintusStocks.GetStockWithDailyEntriesAsync(id);
            if (stock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            return stock;
        }

        public async Task CreateStockAsync(PlıntusStock stock)
        {
            if (string.IsNullOrEmpty(stock.Code))
                throw new ArgumentException("Stok kodu boş olamaz.");

            if (string.IsNullOrEmpty(stock.Name))
                throw new ArgumentException("Stok adı boş olamaz.");

            if (stock.Quantity < 0)
                throw new ArgumentException("Stok miktarı negatif olamaz.");

            // Aynı kodlu stok varlığını kontrol et
            var existingStocks = await _unitOfWork.PlintusStocks.GetStocksByCodeAsync(stock.Code);
            if (existingStocks.Any())
                throw new InvalidOperationException("Bu kodda bir stok zaten mevcut.");

            await _unitOfWork.PlintusStocks.AddAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateStockAsync(PlıntusStock stock)
        {
            var existingStock = await _unitOfWork.PlintusStocks.GetByIdAsync(stock.Id);
            if (existingStock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            if (string.IsNullOrEmpty(stock.Code))
                throw new ArgumentException("Stok kodu boş olamaz.");

            if (string.IsNullOrEmpty(stock.Name))
                throw new ArgumentException("Stok adı boş olamaz.");

            if (stock.Quantity < 0)
                throw new ArgumentException("Stok miktarı negatif olamaz.");

            // Kod değişiyorsa aynı kodlu başka stok varlığını kontrol et
            if (existingStock.Code != stock.Code)
            {
                var existingStocks = await _unitOfWork.PlintusStocks.GetStocksByCodeAsync(stock.Code);
                if (existingStocks.Any())
                    throw new InvalidOperationException("Bu kodda bir stok zaten mevcut.");
            }

            // Mevcut stok bilgilerini güncelle
            existingStock.Code = stock.Code;
            existingStock.Name = stock.Name;
            existingStock.Quantity = stock.Quantity;

            await _unitOfWork.PlintusStocks.UpdateAsync(existingStock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStockAsync(int id)
        {
            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(id);
            if (stock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            // Stokla ilişkili günlük kayıtları kontrol et
            var stockWithEntries = await _unitOfWork.PlintusStocks.GetStockWithDailyEntriesAsync(id);
            if (stockWithEntries.PlıntusDailies.Any())
                throw new InvalidOperationException("Bu stoğa ait günlük üretim kayıtları var. Stok silinemiyor.");

            // Stokla ilişkili satış detaylarını kontrol et
            var salesDetails = await _unitOfWork.SalesDetails.GetDetailsByStockIdAsync(id, "Plıntus");
            if (salesDetails.Any())
                throw new InvalidOperationException("Bu stoğa ait satış kayıtları var. Stok silinemiyor.");

            await _unitOfWork.PlintusStocks.RemoveAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task IncreaseStockQuantityAsync(int stockId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Artırılacak miktar 0'dan büyük olmalıdır.");

            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            stock.Quantity += quantity;
            await _unitOfWork.PlintusStocks.UpdateAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DecreaseStockQuantityAsync(int stockId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Azaltılacak miktar 0'dan büyük olmalıdır.");

            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Plıntus stoğu bulunamadı.");

            if (stock.Quantity < quantity)
                throw new InvalidOperationException("Stok miktarı yetersiz. Mevcut stok: " + stock.Quantity);

            stock.Quantity -= quantity;
            await _unitOfWork.PlintusStocks.UpdateAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetTotalStockQuantityAsync()
        {
            var stocks = await _unitOfWork.PlintusStocks.GetAllAsync();
            return stocks.Sum(s => s.Quantity);
        }

        public async Task<List<PlıntusStock>> GetLowStockItemsAsync(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("Eşik değeri negatif olamaz.");

            var stocks = await _unitOfWork.PlintusStocks.GetAllAsync();
            return stocks.Where(s => s.Quantity <= threshold).ToList();
        }
    }
}
