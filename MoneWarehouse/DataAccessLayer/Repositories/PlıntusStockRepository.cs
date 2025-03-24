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
    public class PlıntusStockRepository : GenericRepository<PlıntusStock>, IPlıntusStockRepository
    {
        public PlıntusStockRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<PlıntusStock>> GetStocksByCodeAsync(string code)
        {
            return await _dbSet.Where(s => s.Code == code).ToListAsync();
        }   

        public async Task<PlıntusStock> GetStockWithDailyEntriesAsync(int stockId)
        {
            return await _dbSet
                .Include(s => s.PlıntusDailies)
                .FirstOrDefaultAsync(s => s.Id == stockId);
        }
    }
}