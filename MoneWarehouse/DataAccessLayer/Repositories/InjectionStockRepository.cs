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
    public class InjectionStockRepository : GenericRepository<InjectionStock>, IInjectionStockRepository
    {
        public InjectionStockRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<InjectionStock>> GetStocksByCodeAsync(string code)
        {
            return await _dbSet.Where(s => s.Code == code).ToListAsync();
        }

        public async Task<InjectionStock> GetStockWithDailyEntriesAsync(int stockId)
        {
            return await _dbSet
                .Include(s => s.InjectionDailies)
                .FirstOrDefaultAsync(s => s.Id == stockId);
        }
    }
}
