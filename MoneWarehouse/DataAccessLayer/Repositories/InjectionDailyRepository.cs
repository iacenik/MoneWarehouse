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
    public class InjectionDailyRepository : GenericRepository<InjectionDaily>, IInjectionDailyRepository
    {
        public InjectionDailyRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(id => id.Date >= startDate && id.Date <= endDate).ToListAsync();
        }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(id => id.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<IEnumerable<InjectionDaily>> GetEntriesByStockIdAsync(int stockId)
        {
            return await _dbSet.Where(id => id.InjectionStockId == stockId).ToListAsync();
        }
    }

}