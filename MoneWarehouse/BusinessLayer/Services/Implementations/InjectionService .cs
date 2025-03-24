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
    public class InjectionService : IInjectionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InjectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Injection>> GetAllInjectionsAsync()
        {
            return await _unitOfWork.Injections.GetAllAsync();
        }

        public async Task<Injection> GetInjectionByIdAsync(int id)
        {
            return await _unitOfWork.Injections.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Injection>> GetInjectionsBySizeAsync(int sizeId)
        {
            // Boyut varlığını kontrol et
            var size = await _unitOfWork.Sizes.GetByIdAsync(sizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            return await _unitOfWork.Injections.GetInjectionsBySizeAsync(sizeId);
        }

        public async Task<IEnumerable<Injection>> GetInjectionsByCodeAsync(int codeId)
        {
            // Kod varlığını kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(codeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            return await _unitOfWork.Injections.GetInjectionsByCodeAsync(codeId);
        }

        public async Task<Injection> GetInjectionWithDailyEntriesAsync(int id)
        {
            var injection = await _unitOfWork.Injections.GetInjectionWithDailyEntriesAsync(id);
            if (injection == null)
                throw new Exception("Enjeksiyon ürünü bulunamadı.");

            return injection;
        }

        public async Task CreateInjectionAsync(Injection injection)
        {
            // Kodu ve boyut bilgisinin geçerliliğini kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(injection.CodeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            var size = await _unitOfWork.Sizes.GetByIdAsync(injection.SizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            // Size ve Code arasındaki ilişkiyi doğrula
            if (code.SizeId != injection.SizeId && code.SizeId.HasValue)
                throw new Exception("Belirtilen kod, boyut ile ilişkili değil.");

            await _unitOfWork.Injections.AddAsync(injection);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateInjectionAsync(Injection injection)
        {
            var existingInjection = await _unitOfWork.Injections.GetByIdAsync(injection.InjectionId);
            if (existingInjection == null)
                throw new Exception("Enjeksiyon ürünü bulunamadı.");

            // Kodu ve boyut bilgisinin geçerliliğini kontrol et
            var code = await _unitOfWork.Codes.GetByIdAsync(injection.CodeId);
            if (code == null)
                throw new Exception("Belirtilen kod bulunamadı.");

            var size = await _unitOfWork.Sizes.GetByIdAsync(injection.SizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            // Size ve Code arasındaki ilişkiyi doğrula
            if (code.SizeId != injection.SizeId && code.SizeId.HasValue)
                throw new Exception("Belirtilen kod, boyut ile ilişkili değil.");

            // Mevcut enjeksiyon bilgilerini güncelle
            existingInjection.SizeId = injection.SizeId;
            existingInjection.CodeId = injection.CodeId;

            await _unitOfWork.Injections.UpdateAsync(existingInjection);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteInjectionAsync(int id)
        {
            var injection = await _unitOfWork.Injections.GetByIdAsync(id);
            if (injection == null)
                throw new Exception("Enjeksiyon ürünü bulunamadı.");

            // Enjeksiyon ürünüyle ilişkili günlük kayıtları kontrol et
            var injectionWithEntries = await _unitOfWork.Injections.GetInjectionWithDailyEntriesAsync(id);
            if (injectionWithEntries.InjectionDailyEntries != null && injectionWithEntries.InjectionDailyEntries.Any())
                throw new InvalidOperationException("Bu enjeksiyon ürününe ait günlük üretim kayıtları var. Ürün silinemiyor.");

            await _unitOfWork.Injections.RemoveAsync(injection);
            await _unitOfWork.CompleteAsync();
        }
    }

}
