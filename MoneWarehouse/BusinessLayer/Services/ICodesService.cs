using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface ICodesService
    {
        Task<IEnumerable<Codes>> GetAllCodesAsync();
        Task<Codes> GetCodeByIdAsync(int id);
        Task<IEnumerable<Codes>> GetCodesBySizeAsync(int sizeId);
        Task CreateCodeAsync(Codes code);
        Task UpdateCodeAsync(Codes code);
        Task DeleteCodeAsync(int id);
    }
}
