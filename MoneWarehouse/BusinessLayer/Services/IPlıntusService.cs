using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IPlıntusService
    {
        Task<IEnumerable<Plıntus>> GetAllPlintusesAsync();
        Task<IEnumerable<Plıntus>> GetActivePlintusesAsync();
        Task<Plıntus> GetPlintusByIdAsync(int id);
        Task<IEnumerable<Plıntus>> GetPlintusesBySizeAsync(int sizeId);
        Task<IEnumerable<Plıntus>> GetPlintusesByCodeAsync(int codeId);
        Task<Plıntus> GetPlintusWithDailyEntriesAsync(int id);
        Task<Plıntus> GetPlintusWithStocksAsync(int id);
        Task CreatePlintusAsync(Plıntus plintus);
        Task UpdatePlintusAsync(Plıntus plintus);
        Task DeletePlintusAsync(int id);
        Task DeactivatePlintusAsync(int id);
    }
}
