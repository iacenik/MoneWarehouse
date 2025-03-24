// IInjectionStockRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IInjectionStockRepository : IGenericRepository<InjectionStock>
    {
        Task<IEnumerable<InjectionStock>> GetStocksByCodeAsync(string code);
        Task<InjectionStock> GetStockWithDailyEntriesAsync(int stockId);
    }

}