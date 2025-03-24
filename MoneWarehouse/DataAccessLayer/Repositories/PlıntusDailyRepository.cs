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
    public class PlıntusDailyRepository : GenericRepository<PlıntusDaily>, IPlıntusDailyRepository
    {
        public PlıntusDailyRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(pd => pd.Date >= startDate && pd.Date <= endDate).ToListAsync();
        }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(pd => pd.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<IEnumerable<PlıntusDaily>> GetEntriesByStockIdAsync(int stockId)
        {
            return await _dbSet.Where(pd => pd.PlıntusStockId == stockId).ToListAsync();
        }
    }
} 