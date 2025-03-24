using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IInjectionRepository : IGenericRepository<Injection>
    {
        Task<IEnumerable<Injection>> GetInjectionsBySizeAsync(int sizeId);
        Task<IEnumerable<Injection>> GetInjectionsByCodeAsync(int codeId);
        Task<Injection> GetInjectionWithDailyEntriesAsync(int injectionId);
    }

}