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
    public class PlıntusRepository : GenericRepository<Plıntus>, IPlıntusRepository
    {
        public PlıntusRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Plıntus>> GetActivePlintusesAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Plıntus>> GetPlintusesBySizeAsync(int sizeId)
        {
            return await _dbSet.Where(p => p.SizeId == sizeId).ToListAsync();
        }

        public async Task<IEnumerable<Plıntus>> GetPlintusesByCodeAsync(int codeId)
        {
            return await _dbSet.Where(p => p.CodeId == codeId).ToListAsync();
        }

        public async Task<Plıntus> GetPlintusWithDailyEntriesAsync(int plintusId)
        {
            return await _dbSet
                .Include(p => p.PlıntusDailyEntries)
                .FirstOrDefaultAsync(p => p.PlıntusId == plintusId);
        }

        public async Task<Plıntus> GetPlintusWithStocksAsync(int plintusId)
        {
            return await _dbSet
                .Include(p => p.PlıntusStocks)
                .FirstOrDefaultAsync(p => p.PlıntusId == plintusId);
        }
    }
}