using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface ISizeService
    {
        Task<IEnumerable<Size>> GetAllSizesAsync();
        Task<Size> GetSizeByIdAsync(int id);
        Task<IEnumerable<Size>> GetSizesByTypeAsync(string sizeType);
        Task<IEnumerable<Size>> GetActiveSizesAsync();
        Task<Size> GetSizeWithCodesAsync(int id);
        Task CreateSizeAsync(Size size);
        Task UpdateSizeAsync(Size size);
        Task DeleteSizeAsync(int id);
        Task DeactivateSizeAsync(int id);
    }
}
