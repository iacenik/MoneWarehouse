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
    public class CodesService : ICodesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CodesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Codes>> GetAllCodesAsync()
        {
            return await _unitOfWork.Codes.GetAllAsync();
        }

        public async Task<Codes> GetCodeByIdAsync(int id)
        {
            return await _unitOfWork.Codes.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Codes>> GetCodesBySizeAsync(int sizeId)
        {
            // Boyut varlığını kontrol et
            var size = await _unitOfWork.Sizes.GetByIdAsync(sizeId);
            if (size == null)
                throw new Exception("Belirtilen boyut bulunamadı.");

            return await _unitOfWork.Codes.GetCodesBySizeAsync(sizeId);
        }

        public async Task CreateCodeAsync(Codes code)
        {
            if (string.IsNullOrEmpty(code.Code))
                throw new ArgumentException("Kod boş olamaz.");

            if (string.IsNullOrEmpty(code.CodeName))
                throw new ArgumentException("Kod adı boş olamaz.");

            // Size ID varsa varlığını kontrol et
            if (code.SizeId.HasValue)
            {
                var size = await _unitOfWork.Sizes.GetByIdAsync(code.SizeId.Value);
                if (size == null)
                    throw new Exception("Belirtilen boyut bulunamadı.");
            }

            await _unitOfWork.Codes.AddAsync(code);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCodeAsync(Codes code)
        {
            var existingCode = await _unitOfWork.Codes.GetByIdAsync(code.CodeId);
            if (existingCode == null)
                throw new Exception("Kod bulunamadı.");

            if (string.IsNullOrEmpty(code.Code))
                throw new ArgumentException("Kod boş olamaz.");

            if (string.IsNullOrEmpty(code.CodeName))
                throw new ArgumentException("Kod adı boş olamaz.");

            // Size ID varsa varlığını kontrol et
            if (code.SizeId.HasValue)
            {
                var size = await _unitOfWork.Sizes.GetByIdAsync(code.SizeId.Value);
                if (size == null)
                    throw new Exception("Belirtilen boyut bulunamadı.");
            }

            // Mevcut kod bilgilerini güncelle
            existingCode.Code = code.Code;
            existingCode.CodeName = code.CodeName;
            existingCode.SizeId = code.SizeId;

            await _unitOfWork.Codes.UpdateAsync(existingCode);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCodeAsync(int id)
        {
            var code = await _unitOfWork.Codes.GetByIdAsync(id);
            if (code == null)
                throw new Exception("Kod bulunamadı.");

            // Koda bağlı ürünleri kontrol et (Injection ve Plıntus)
            var injections = await _unitOfWork.Injections.GetInjectionsByCodeAsync(id);
            if (injections.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu kodu kullanan enjeksiyon ürünleri var. Kod silinemiyor.");

            var plintuses = await _unitOfWork.Plintuses.GetPlintusesByCodeAsync(id);
            if (plintuses.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu kodu kullanan plintus ürünleri var. Kod silinemiyor.");

            await _unitOfWork.Codes.RemoveAsync(code);
            await _unitOfWork.CompleteAsync();
        }
    }
}