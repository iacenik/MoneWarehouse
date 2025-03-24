using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class InjectionStockService : IInjectionStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InjectionStockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<InjectionStock>> GetAllStocksAsync()
        {
            return await _unitOfWork.InjectionStocks.GetAllAsync();
        }

        public async Task<InjectionStock> GetStockByIdAsync(int id)
        {
            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(id);
            if (stock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            return stock;
        }

        public async Task<IEnumerable<InjectionStock>> GetStocksByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Kod bilgisi boş olamaz.");

            return await _unitOfWork.InjectionStocks.GetStocksByCodeAsync(code);
        }

        public async Task<InjectionStock> GetStockWithDailyEntriesAsync(int id)
        {
            var stock = await _unitOfWork.InjectionStocks.GetStockWithDailyEntriesAsync(id);
            if (stock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            return stock;
        }

        public async Task CreateStockAsync(InjectionStock stock)
        {
            if (string.IsNullOrEmpty(stock.Code))
                throw new ArgumentException("Stok kodu boş olamaz.");

            if (string.IsNullOrEmpty(stock.Name))
                throw new ArgumentException("Stok adı boş olamaz.");

            if (stock.Quantity < 0)
                throw new ArgumentException("Stok miktarı negatif olamaz.");

            // Aynı kodlu stok varlığını kontrol et
            var existingStocks = await _unitOfWork.InjectionStocks.GetStocksByCodeAsync(stock.Code);
            if (existingStocks.Any())
                throw new InvalidOperationException("Bu kodda bir stok zaten mevcut.");

            await _unitOfWork.InjectionStocks.AddAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateStockAsync(InjectionStock stock)
        {
            var existingStock = await _unitOfWork.InjectionStocks.GetByIdAsync(stock.Id);
            if (existingStock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            if (string.IsNullOrEmpty(stock.Code))
                throw new ArgumentException("Stok kodu boş olamaz.");

            if (string.IsNullOrEmpty(stock.Name))
                throw new ArgumentException("Stok adı boş olamaz.");

            if (stock.Quantity < 0)
                throw new ArgumentException("Stok miktarı negatif olamaz.");

            // Kod değişiyorsa aynı kodlu başka stok varlığını kontrol et
            if (existingStock.Code != stock.Code)
            {
                var existingStocks = await _unitOfWork.InjectionStocks.GetStocksByCodeAsync(stock.Code);
                if (existingStocks.Any())
                    throw new InvalidOperationException("Bu kodda bir stok zaten mevcut.");
            }

            // Mevcut stok bilgilerini güncelle
            existingStock.Code = stock.Code;
            existingStock.Name = stock.Name;
            existingStock.Quantity = stock.Quantity;

            await _unitOfWork.InjectionStocks.UpdateAsync(existingStock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStockAsync(int id)
        {
            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(id);
            if (stock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            // Stokla ilişkili günlük kayıtları kontrol et
            var stockWithEntries = await _unitOfWork.InjectionStocks.GetStockWithDailyEntriesAsync(id);
            if (stockWithEntries.InjectionDailies.Any())
                throw new InvalidOperationException("Bu stoğa ait günlük üretim kayıtları var. Stok silinemiyor.");

            // Stokla ilişkili satış detaylarını kontrol et
            var salesDetails = await _unitOfWork.SalesDetails.GetDetailsByStockIdAsync(id, "Injection");
            if (salesDetails.Any())
                throw new InvalidOperationException("Bu stoğa ait satış kayıtları var. Stok silinemiyor.");

            await _unitOfWork.InjectionStocks.RemoveAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task IncreaseStockQuantityAsync(int stockId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Artırılacak miktar 0'dan büyük olmalıdır.");

            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            stock.Quantity += quantity;
            await _unitOfWork.InjectionStocks.UpdateAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DecreaseStockQuantityAsync(int stockId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Azaltılacak miktar 0'dan büyük olmalıdır.");

            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Enjeksiyon stoğu bulunamadı.");

            if (stock.Quantity < quantity)
                throw new InvalidOperationException("Stok miktarı yetersiz. Mevcut stok: " + stock.Quantity);

            stock.Quantity -= quantity;
            await _unitOfWork.InjectionStocks.UpdateAsync(stock);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetTotalStockQuantityAsync()
        {
            var stocks = await _unitOfWork.InjectionStocks.GetAllAsync();
            return stocks.Sum(s => s.Quantity);
        }

        public async Task<List<InjectionStock>> GetLowStockItemsAsync(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("Eşik değeri negatif olamaz.");

            var stocks = await _unitOfWork.InjectionStocks.GetAllAsync();
            return stocks.Where(s => s.Quantity <= threshold).ToList();
        }
    }
}
