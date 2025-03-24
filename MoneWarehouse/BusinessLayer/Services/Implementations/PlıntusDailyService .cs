using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class PlıntusDailyService : IPlıntusDailyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlıntusStockService _plintusStockService;

        public PlıntusDailyService(IUnitOfWork unitOfWork, IPlıntusStockService plintusStockService)
        {
            _unitOfWork = unitOfWork;
            _plintusStockService = plintusStockService;
        }

        public async Task<IEnumerable<PlıntusDaily>> GetAllEntriesAsync()
        {
            return await _unitOfWork.PlintusDailies.GetAllAsync();
        }

        public async Task<PlıntusDaily> GetEntryByIdAsync(int id)
        {
            var entry = await _unitOfWork.PlintusDailies.GetByIdAsync(id);
            if (entry == null)
                throw new Exception("Günlük plıntus kaydı bulunamadı.");

            return entry;
        }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            return await _unitOfWork.PlintusDailies.GetEntriesByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByEmployeeAsync(int employeeId)
        {
            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            return await _unitOfWork.PlintusDailies.GetEntriesByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByStockIdAsync(int stockId)
        {
            // Stok varlığını kontrol et
            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Belirtilen stok bulunamadı.");

            return await _unitOfWork.PlintusDailies.GetEntriesByStockIdAsync(stockId);
        }

        public async Task CreateEntryAsync(PlıntusDaily entry)
        {
            // Stok varlığını kontrol et
            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(entry.PlıntusStockId);
            if (stock == null)
                throw new Exception("Belirtilen stok bulunamadı.");

            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(entry.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            if (!employee.IsActive)
                throw new InvalidOperationException("Bu çalışan aktif değil. Üretim kaydı eklenemez.");

            if (entry.Quantity <= 0)
                throw new ArgumentException("Üretim miktarı 0'dan büyük olmalıdır.");

            if (entry.Date > DateTime.Now)
                throw new ArgumentException("Gelecek tarihli üretim kaydı eklenemez.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Günlük üretim kaydı ekle
                await _unitOfWork.PlintusDailies.AddAsync(entry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını artır
                await _plintusStockService.IncreaseStockQuantityAsync(entry.PlıntusStockId, entry.Quantity);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Üretim kaydı eklenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task UpdateEntryAsync(PlıntusDaily entry, int oldQuantity)
        {
            var existingEntry = await _unitOfWork.PlintusDailies.GetByIdAsync(entry.Id);
            if (existingEntry == null)
                throw new Exception("Günlük plıntus kaydı bulunamadı.");

            // Stok varlığını kontrol et
            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(entry.PlıntusStockId);
            if (stock == null)
                throw new Exception("Belirtilen stok bulunamadı.");

            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(entry.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            if (entry.Quantity <= 0)
                throw new ArgumentException("Üretim miktarı 0'dan büyük olmalıdır.");

            if (entry.Date > DateTime.Now)
                throw new ArgumentException("Gelecek tarihli üretim kaydı eklenemez.");

            // Eski kayıt ile yeni kayıt arasındaki miktar farkını hesapla
            int quantityDifference = entry.Quantity - oldQuantity;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Günlük üretim kaydını güncelle
                existingEntry.PlıntusStockId = entry.PlıntusStockId;
                existingEntry.EmployeeId = entry.EmployeeId;
                existingEntry.Quantity = entry.Quantity;
                existingEntry.Date = entry.Date;
                existingEntry.Description = entry.Description;

                await _unitOfWork.PlintusDailies.UpdateAsync(existingEntry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını güncelle (fark pozitifse artır, negatifse azalt)
                if (quantityDifference > 0)
                {
                    await _plintusStockService.IncreaseStockQuantityAsync(entry.PlıntusStockId, quantityDifference);
                }
                else if (quantityDifference < 0)
                {
                    await _plintusStockService.DecreaseStockQuantityAsync(entry.PlıntusStockId, Math.Abs(quantityDifference));
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Üretim kaydı güncellenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task DeleteEntryAsync(int id)
        {
            var entry = await _unitOfWork.PlintusDailies.GetByIdAsync(id);
            if (entry == null)
                throw new Exception("Günlük plıntus kaydı bulunamadı.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Günlük üretim kaydını sil
                await _unitOfWork.PlintusDailies.RemoveAsync(entry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını azalt (üretim kaydı silindiği için)
                await _plintusStockService.DecreaseStockQuantityAsync(entry.PlıntusStockId, entry.Quantity);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Üretim kaydı silinirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<int> GetTotalProductionByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            var entries = await _unitOfWork.PlintusDailies.GetEntriesByDateRangeAsync(startDate, endDate);
            return entries.Sum(e => e.Quantity);
        }

        public async Task<Dictionary<int, int>> GetProductionByEmployeeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            var entries = await _unitOfWork.PlintusDailies.GetEntriesByDateRangeAsync(startDate, endDate);
            return entries
                .GroupBy(e => e.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Quantity));
        }
    }

}
