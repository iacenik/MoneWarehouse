using DataAccessLayer;
using DataAccessLayer.Repositories.Interfaces;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class InjectionDailyService : IInjectionDailyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInjectionStockService _injectionStockService;

        public InjectionDailyService(IUnitOfWork unitOfWork, IInjectionStockService injectionStockService)
        {
            _unitOfWork = unitOfWork;
            _injectionStockService = injectionStockService;
        }

        public async Task<IEnumerable<InjectionDaily>> GetAllEntriesAsync()
        {
            return await _unitOfWork.InjectionDailies.GetAllAsync();
        }

        public async Task<InjectionDaily> GetEntryByIdAsync(int id)
        {
            var entry = await _unitOfWork.InjectionDailies.GetByIdAsync(id);
            if (entry == null)
                throw new Exception("Günlük enjeksiyon kaydı bulunamadı.");

            return entry;
        }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            return await _unitOfWork.InjectionDailies.GetEntriesByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByEmployeeAsync(int employeeId)
        {
            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            return await _unitOfWork.InjectionDailies.GetEntriesByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByStockIdAsync(int stockId)
        {
            // Stok varlığını kontrol et
            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(stockId);
            if (stock == null)
                throw new Exception("Belirtilen stok bulunamadı.");

            return await _unitOfWork.InjectionDailies.GetEntriesByStockIdAsync(stockId);
        }

        public async Task CreateEntryAsync(InjectionDaily entry)
        {
            // Stok varlığını kontrol et
            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(entry.InjectionStockId);
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
                await _unitOfWork.InjectionDailies.AddAsync(entry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını artır
                await _injectionStockService.IncreaseStockQuantityAsync(entry.InjectionStockId, entry.Quantity);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Üretim kaydı eklenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task UpdateEntryAsync(InjectionDaily entry, int oldQuantity)
        {
            var existingEntry = await _unitOfWork.InjectionDailies.GetByIdAsync(entry.Id);
            if (existingEntry == null)
                throw new Exception("Günlük enjeksiyon kaydı bulunamadı.");

            // Stok varlığını kontrol et
            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(entry.InjectionStockId);
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
                existingEntry.InjectionStockId = entry.InjectionStockId;
                existingEntry.EmployeeId = entry.EmployeeId;
                existingEntry.Quantity = entry.Quantity;
                existingEntry.Date = entry.Date;
                existingEntry.Description = entry.Description;

                await _unitOfWork.InjectionDailies.UpdateAsync(existingEntry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını güncelle (fark pozitifse artır, negatifse azalt)
                if (quantityDifference > 0)
                {
                    await _injectionStockService.IncreaseStockQuantityAsync(entry.InjectionStockId, quantityDifference);
                }
                else if (quantityDifference < 0)
                {
                    await _injectionStockService.DecreaseStockQuantityAsync(entry.InjectionStockId, Math.Abs(quantityDifference));
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
            var entry = await _unitOfWork.InjectionDailies.GetByIdAsync(id);
            if (entry == null)
                throw new Exception("Günlük enjeksiyon kaydı bulunamadı.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Günlük üretim kaydını sil
                await _unitOfWork.InjectionDailies.RemoveAsync(entry);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını azalt (üretim kaydı silindiği için)
                await _injectionStockService.DecreaseStockQuantityAsync(entry.InjectionStockId, entry.Quantity);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Üretim kaydı silinirken bir hata oluştu: " + ex.Message);
            }
        }
    }


}
