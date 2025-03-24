using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ISizeRepository : IGenericRepository<Size>
    {
        Task<IEnumerable<Size>> GetSizesByTypeAsync(string sizeType);
        Task<Size> GetSizeWithCodesAsync(int sizeId);
        Task<IEnumerable<Size>> GetActiveSizesAsync();
    }
} 