using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IFormulaService
    {
        Task<IEnumerable<Formula>> GetAllFormulasAsync();
        Task<Formula> GetFormulaByIdAsync(int id);
        Task<Formula> GetFormulaByNameAsync(string name);
        Task CreateFormulaAsync(Formula formula);
        Task UpdateFormulaAsync(Formula formula);
        Task DeleteFormulaAsync(int id);
    }
}
