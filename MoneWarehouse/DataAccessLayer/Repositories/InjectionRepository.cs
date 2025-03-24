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
    public class InjectionRepository : GenericRepository<Injection>, IInjectionRepository
    {
        public InjectionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Injection>> GetInjectionsBySizeAsync(int sizeId)
        {
            return await _dbSet.Where(i => i.SizeId == sizeId).ToListAsync();
        }

        public async Task<IEnumerable<Injection>> GetInjectionsByCodeAsync(int codeId)
        {
            return await _dbSet.Where(i => i.CodeId == codeId).ToListAsync();
        }

        public async Task<Injection> GetInjectionWithDailyEntriesAsync(int injectionId)
        {
            return await _dbSet
                .Include(i => i.InjectionDailyEntries)
                .FirstOrDefaultAsync(i => i.InjectionId == injectionId);
        }
    }

}
