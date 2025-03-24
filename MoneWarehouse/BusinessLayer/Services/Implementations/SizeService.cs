using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Size>> GetAllSizesAsync()
        {
            return await _unitOfWork.Sizes.GetAllAsync();
        }

        public async Task<Size> GetSizeByIdAsync(int id)
        {
            var size = await _unitOfWork.Sizes.GetByIdAsync(id);
            if (size == null)
                throw new Exception("Boyut bulunamadı.");

            return size;
        }

        public async Task<IEnumerable<Size>> GetSizesByTypeAsync(string sizeType)
        {
            if (string.IsNullOrEmpty(sizeType))
                throw new ArgumentException("Boyut tipi boş olamaz.");

            return await _unitOfWork.Sizes.GetSizesByTypeAsync(sizeType);
        }

        public async Task<IEnumerable<Size>> GetActiveSizesAsync()
        {
            return await _unitOfWork.Sizes.GetActiveSizesAsync();
        }

        public async Task<Size> GetSizeWithCodesAsync(int id)
        {
            var size = await _unitOfWork.Sizes.GetSizeWithCodesAsync(id);
            if (size == null)
                throw new Exception("Boyut bulunamadı.");

            return size;
        }

        public async Task CreateSizeAsync(Size size)
        {
            if (string.IsNullOrEmpty(size.SizeName))
                throw new ArgumentException("Boyut adı boş olamaz.");

            if (string.IsNullOrEmpty(size.SizeType))
                throw new ArgumentException("Boyut tipi boş olamaz.");

            // Aynı isimli boyut varlığını kontrol et
            var existingSizes = await _unitOfWork.Sizes.FindAsync(s => s.SizeName == size.SizeName && s.SizeType == size.SizeType);
            if (existingSizes.Any())
                throw new InvalidOperationException("Bu isimde ve tipte bir boyut zaten mevcut.");

            size.CreatedDate = DateTime.Now;
            await _unitOfWork.Sizes.AddAsync(size);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateSizeAsync(Size size)
        {
            var existingSize = await _unitOfWork.Sizes.GetByIdAsync(size.SizeId);
            if (existingSize == null)
                throw new Exception("Boyut bulunamadı.");

            if (string.IsNullOrEmpty(size.SizeName))
                throw new ArgumentException("Boyut adı boş olamaz.");

            if (string.IsNullOrEmpty(size.SizeType))
                throw new ArgumentException("Boyut tipi boş olamaz.");

            // İsim veya tip değişiyorsa aynı isimli ve tipli başka boyut varlığını kontrol et
            if (existingSize.SizeName != size.SizeName || existingSize.SizeType != size.SizeType)
            {
                var existingSizes = await _unitOfWork.Sizes.FindAsync(s => s.SizeName == size.SizeName && s.SizeType == size.SizeType && s.SizeId != size.SizeId);
                if (existingSizes.Any())
                    throw new InvalidOperationException("Bu isimde ve tipte bir boyut zaten mevcut.");
            }

            // Mevcut boyut bilgilerini güncelle
            existingSize.SizeName = size.SizeName;
            existingSize.SizeType = size.SizeType;
            existingSize.Description = size.Description;
            existingSize.IsActive = size.IsActive;
            existingSize.UpdatedDate = DateTime.Now;

            await _unitOfWork.Sizes.UpdateAsync(existingSize);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteSizeAsync(int id)
        {
            var size = await _unitOfWork.Sizes.GetByIdAsync(id);
            if (size == null)
                throw new Exception("Boyut bulunamadı.");

            // Boyutla ilişkili kodları kontrol et
            var sizeWithCodes = await _unitOfWork.Sizes.GetSizeWithCodesAsync(id);
            if (sizeWithCodes.Codes != null && sizeWithCodes.Codes.Any())
                throw new InvalidOperationException("Bu boyutla ilişkili kodlar var. Boyut silinemiyor.");

            // Enjeksiyon ve plıntus ürünlerini kontrol et
            var injections = await _unitOfWork.Injections.GetInjectionsBySizeAsync(id);
            if (injections != null && injections.Any())
                throw new InvalidOperationException("Bu boyutla ilişkili enjeksiyon ürünleri var. Boyut silinemiyor.");

            var plintuses = await _unitOfWork.Plintuses.GetPlintusesBySizeAsync(id);
            if (plintuses != null && plintuses.Any())
                throw new InvalidOperationException("Bu boyutla ilişkili plıntus ürünleri var. Boyut silinemiyor.");

            await _unitOfWork.Sizes.RemoveAsync(size);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeactivateSizeAsync(int id)
        {
            var size = await _unitOfWork.Sizes.GetByIdAsync(id);
            if (size == null)
                throw new Exception("Boyut bulunamadı.");

            // İlişkili ürünler için uyarı kontrolü
            var sizeWithCodes = await _unitOfWork.Sizes.GetSizeWithCodesAsync(id);
            if (sizeWithCodes.Codes != null && sizeWithCodes.Codes.Any())
            {
                foreach (var code in sizeWithCodes.Codes)
                {
                    // İlgili kodu kullanan aktif ürünleri kontrol et
                    var injections = await _unitOfWork.Injections.GetInjectionsByCodeAsync(code.CodeId);
                    if (injections != null && injections.Any())
                    {
                        throw new InvalidOperationException("Bu boyutun kodları aktif enjeksiyon ürünleri tarafından kullanılmaktadır. Devre dışı bırakılamaz.");
                    }

                    var plintuses = await _unitOfWork.Plintuses.GetPlintusesByCodeAsync(code.CodeId);
                    if (plintuses != null && plintuses.Any(p => p.IsActive))
                    {
                        throw new InvalidOperationException("Bu boyutun kodları aktif plıntus ürünleri tarafından kullanılmaktadır. Devre dışı bırakılamaz.");
                    }
                }
            }

            size.IsActive = false;
            size.UpdatedDate = DateTime.Now;

            await _unitOfWork.Sizes.UpdateAsync(size);
            await _unitOfWork.CompleteAsync();
        }
    }
}