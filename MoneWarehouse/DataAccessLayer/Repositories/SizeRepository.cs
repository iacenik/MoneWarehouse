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
    public class SizeRepository : GenericRepository<Size>, ISizeRepository
    {
        public SizeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Size>> GetSizesByTypeAsync(string sizeType)
        {
            return await _dbSet.Where(s => s.SizeType == sizeType).ToListAsync();
        }

        public async Task<Size> GetSizeWithCodesAsync(int sizeId)
        {
            return await _dbSet
                .Include(s => s.Codes)
                .FirstOrDefaultAsync(s => s.SizeId == sizeId);
        }

        public async Task<IEnumerable<Size>> GetActiveSizesAsync()
        {
            return await _dbSet.Where(s => s.IsActive).ToListAsync();
        }
    }
} 