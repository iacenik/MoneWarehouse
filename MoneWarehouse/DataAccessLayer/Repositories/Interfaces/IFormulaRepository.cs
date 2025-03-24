using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IFormulaRepository : IGenericRepository<Formula>
    {
        Task<Formula> GetFormulaByNameAsync(string name);
    }
} 