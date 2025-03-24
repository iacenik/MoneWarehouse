using DataAccessLayer;
using EntityLayer.Entities;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Material>> GetAllMaterialsAsync()
        {
            return await _unitOfWork.Materials.GetAllAsync();
        }

        public async Task<Material> GetMaterialByIdAsync(int id)
        {
            var material = await _unitOfWork.Materials.GetByIdAsync(id);
            if (material == null)
                throw new Exception("Malzeme bulunamadı.");

            return material;
        }

        public async Task<IEnumerable<Material>> GetMaterialsByUnitAsync(MaterialUnit unit)
        {
            if (!Enum.IsDefined(typeof(MaterialUnit), unit))
                throw new ArgumentException("Geçersiz birim tipi.");

            return await _unitOfWork.Materials.GetMaterialsByUnitAsync(unit);
        }

        public async Task CreateMaterialAsync(Material material)
        {
            if (string.IsNullOrEmpty(material.Name))
                throw new ArgumentException("Malzeme adı boş olamaz.");

            if (material.Quantity <= 0)
                throw new ArgumentException("Malzeme miktarı 0'dan büyük olmalıdır.");

            // Enum değerinin geçerliliğini kontrol et
            if (!Enum.IsDefined(typeof(MaterialUnit), material.Unit))
                throw new ArgumentException("Geçersiz birim tipi.");

            // Aynı isimli malzeme varlığını kontrol et
            var existingMaterials = await _unitOfWork.Materials.FindAsync(m => m.Name == material.Name);
            if (existingMaterials.Any())
                throw new InvalidOperationException("Bu isimde bir malzeme zaten mevcut.");

            await _unitOfWork.Materials.AddAsync(material);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateMaterialAsync(Material material)
        {
            var existingMaterial = await _unitOfWork.Materials.GetByIdAsync(material.MaterialId);
            if (existingMaterial == null)
                throw new Exception("Malzeme bulunamadı.");

            if (string.IsNullOrEmpty(material.Name))
                throw new ArgumentException("Malzeme adı boş olamaz.");

            if (material.Quantity < 0)
                throw new ArgumentException("Malzeme miktarı negatif olamaz.");

            // Enum değerinin geçerliliğini kontrol et
            if (!Enum.IsDefined(typeof(MaterialUnit), material.Unit))
                throw new ArgumentException("Geçersiz birim tipi.");

            // İsim değişiyorsa aynı isimli başka malzeme varlığını kontrol et
            if (existingMaterial.Name != material.Name)
            {
                var nameExists = await _unitOfWork.Materials.FindAsync(m => m.Name == material.Name);
                if (nameExists.Any())
                    throw new InvalidOperationException("Bu isimde bir malzeme zaten mevcut.");
            }

            // Mevcut malzeme bilgilerini güncelle
            existingMaterial.Name = material.Name;
            existingMaterial.Quantity = material.Quantity;
            existingMaterial.Unit = material.Unit;

            await _unitOfWork.Materials.UpdateAsync(existingMaterial);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteMaterialAsync(int id)
        {
            var material = await _unitOfWork.Materials.GetByIdAsync(id);
            if (material == null)
                throw new Exception("Malzeme bulunamadı.");

            // Malzemeyi kullanılan yerleri kontrol etme mantığı buraya eklenebilir
            // Örneğin, malzeme üretimde kullanılıyorsa silinmesini engelleme

            await _unitOfWork.Materials.RemoveAsync(material);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<Material>> GetLowQuantityMaterialsAsync(double threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("Eşik değeri negatif olamaz.");

            var materials = await _unitOfWork.Materials.GetAllAsync();
            return materials.Where(m => m.Quantity <= threshold).ToList();
        }
    }
}
