using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories.Interfaces;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class FormulaRepository : GenericRepository<Formula>, IFormulaRepository
    {
        public FormulaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Formula> GetFormulaByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.FormulaName == name);
        }
    }
} 