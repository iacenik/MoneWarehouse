// IPlintusStockRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IPlıntusStockRepository : IGenericRepository<PlıntusStock>
    {
        Task<IEnumerable<PlıntusStock>> GetStocksByCodeAsync(string code);
        Task<PlıntusStock> GetStockWithDailyEntriesAsync(int stockId);
    }
}