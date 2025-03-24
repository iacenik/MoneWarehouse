using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ICodesRepository : IGenericRepository<Codes>
    {
        Task<IEnumerable<Codes>> GetCodesBySizeAsync(int sizeId);
    }
} 