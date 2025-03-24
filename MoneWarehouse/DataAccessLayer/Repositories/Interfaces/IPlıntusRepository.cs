using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IPlıntusRepository : IGenericRepository<Plıntus>
    {
        Task<IEnumerable<Plıntus>> GetActivePlintusesAsync();
        Task<IEnumerable<Plıntus>> GetPlintusesBySizeAsync(int sizeId);
        Task<IEnumerable<Plıntus>> GetPlintusesByCodeAsync(int codeId);
        Task<Plıntus> GetPlintusWithDailyEntriesAsync(int plintusId);
        Task<Plıntus> GetPlintusWithStocksAsync(int plintusId);
    }
}