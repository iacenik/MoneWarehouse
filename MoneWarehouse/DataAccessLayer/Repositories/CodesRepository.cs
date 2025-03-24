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
    public class CodesRepository : GenericRepository<Codes>, ICodesRepository
    {
        public CodesRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Codes>> GetCodesBySizeAsync(int sizeId)
        {
            return await _dbSet.Where(c => c.SizeId == sizeId).ToListAsync();
        }
    }

}