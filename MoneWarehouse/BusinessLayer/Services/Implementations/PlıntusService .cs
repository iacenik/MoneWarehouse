using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class PlıntusService : IPlıntusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlıntusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Plıntus>> GetAllPlintusesAsync()
        {
            return await _unitOfWork.Plintuses.GetAllAsync();
        }

        public async Task<IEnumerable<Plıntus>> GetActivePlintusesAsync()
        {
            return await _unitOfWork.Plintuses.GetActivePlintusesAsync();
        }

        public async Task<Plıntus> GetPlintusByIdAsync(int id)
        {
            var plintus = await _unitOfWork.Plintuses.GetByIdAsync(id);
            if (plintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            return plintus;
        }

        public async Task<IEnumerable<Plıntus>> GetPlintusesBySizeAsync(int sizeId)
        {
            // Boyut varlığını kontrol et
            var size = await _unitOfWork.Sizes.GetByIdAsync(sizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            return await _unitOfWork.Plintuses.GetPlintusesBySizeAsync(sizeId);
        }

        public async Task<IEnumerable<Plıntus>> GetPlintusesByCodeAsync(int codeId)
        {
            // Kod varlığını kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(codeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            return await _unitOfWork.Plintuses.GetPlintusesByCodeAsync(codeId);
        }

        public async Task<Plıntus> GetPlintusWithDailyEntriesAsync(int id)
        {
            var plintus = await _unitOfWork.Plintuses.GetPlintusWithDailyEntriesAsync(id);
            if (plintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            return plintus;
        }

        public async Task<Plıntus> GetPlintusWithStocksAsync(int id)
        {
            var plintus = await _unitOfWork.Plintuses.GetPlintusWithStocksAsync(id);
            if (plintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            return plintus;
        }

        public async Task CreatePlintusAsync(Plıntus plintus)
        {
            if (string.IsNullOrEmpty(plintus.Code))
                throw new ArgumentException("Plıntus kodu boş olamaz.");

            if (string.IsNullOrEmpty(plintus.Name))
                throw new ArgumentException("Plıntus adı boş olamaz.");

            // Kodu ve boyut bilgisinin geçerliliğini kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(plintus.CodeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            var size = await _unitOfWork.Sizes.GetByIdAsync(plintus.SizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            // Size ve Code arasındaki ilişkiyi doğrula
            if (code.SizeId != plintus.SizeId && code.SizeId.HasValue)
                throw new Exception("Belirtilen kod, boyut ile ilişkili değil.");

            // Aynı kodlu plıntus varlığını kontrol et
            var existingPlintuses = await _unitOfWork.Plintuses.FindAsync(p => p.Code == plintus.Code);
            if (existingPlintuses.Any())
                throw new InvalidOperationException("Bu kodda bir plıntus ürünü zaten mevcut.");

            plintus.CreatedDate = DateTime.Now;
            plintus.IsActive = true;

            await _unitOfWork.Plintuses.AddAsync(plintus);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdatePlintusAsync(Plıntus plintus)
        {
            var existingPlintus = await _unitOfWork.Plintuses.GetByIdAsync(plintus.PlıntusId);
            if (existingPlintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            if (string.IsNullOrEmpty(plintus.Code))
                throw new ArgumentException("Plıntus kodu boş olamaz.");

            if (string.IsNullOrEmpty(plintus.Name))
                throw new ArgumentException("Plıntus adı boş olamaz.");

            // Kodu ve boyut bilgisinin geçerliliğini kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(plintus.CodeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            var size = await _unitOfWork.Sizes.GetByIdAsync(plintus.SizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            // Size ve Code arasındaki ilişkiyi doğrula
            if (code.SizeId != plintus.SizeId && code.SizeId.HasValue)
                throw new Exception("Belirtilen kod, boyut ile ilişkili değil.");

            // Kod değişiyorsa aynı kodlu başka plıntus varlığını kontrol et
            if (existingPlintus.Code != plintus.Code)
            {
                var existingPlintuses = await _unitOfWork.Plintuses.FindAsync(p => p.Code == plintus.Code);
                if (existingPlintuses.Any())
                    throw new InvalidOperationException("Bu kodda bir plıntus ürünü zaten mevcut.");
            }

            // Mevcut plıntus bilgilerini güncelle
            existingPlintus.Code = plintus.Code;
            existingPlintus.Name = plintus.Name;
            existingPlintus.SizeId = plintus.SizeId;
            existingPlintus.CodeId = plintus.CodeId;
            existingPlintus.IsActive = plintus.IsActive;
            existingPlintus.LastModifiedDate = DateTime.Now;

            await _unitOfWork.Plintuses.UpdateAsync(existingPlintus);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeletePlintusAsync(int id)
        {
            var plintus = await _unitOfWork.Plintuses.GetByIdAsync(id);
            if (plintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            // Plıntus ürünüyle ilişkili günlük kayıtları kontrol et
            var plintusWithEntries = await _unitOfWork.Plintuses.GetPlintusWithDailyEntriesAsync(id);
            if (plintusWithEntries.PlıntusDailyEntries != null && plintusWithEntries.PlıntusDailyEntries.Any())
                throw new InvalidOperationException("Bu plıntus ürününe ait günlük üretim kayıtları var. Ürün silinemiyor.");

            // Plıntus ürünüyle ilişkili stokları kontrol et
            var plintusWithStocks = await _unitOfWork.Plintuses.GetPlintusWithStocksAsync(id);
            if (plintusWithStocks.PlıntusStocks != null && plintusWithStocks.PlıntusStocks.Any())
                throw new InvalidOperationException("Bu plıntus ürününe ait stok kayıtları var. Ürün silinemiyor.");

            await _unitOfWork.Plintuses.RemoveAsync(plintus);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeactivatePlintusAsync(int id)
        {
            var plintus = await _unitOfWork.Plintuses.GetByIdAsync(id);
            if (plintus == null)
                throw new Exception("Plıntus ürünü bulunamadı.");

            plintus.IsActive = false;
            plintus.LastModifiedDate = DateTime.Now;

            await _unitOfWork.Plintuses.UpdateAsync(plintus);
            await _unitOfWork.CompleteAsync();
        }
    }
}
