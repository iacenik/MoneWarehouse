using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IInjectionService
    {
        Task<IEnumerable<Injection>> GetAllInjectionsAsync();
        Task<Injection> GetInjectionByIdAsync(int id);
        Task<IEnumerable<Injection>> GetInjectionsBySizeAsync(int sizeId);
        Task<IEnumerable<Injection>> GetInjectionsByCodeAsync(int codeId);
        Task<Injection> GetInjectionWithDailyEntriesAsync(int id);
        Task CreateInjectionAsync(Injection injection);
        Task UpdateInjectionAsync(Injection injection);
        Task DeleteInjectionAsync(int id);
    }
}
