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
    public class SalesRepository : GenericRepository<Sales>, ISalesRepository
    {
        public SalesRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Sales>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(s => s.SalesDate >= startDate && s.SalesDate <= endDate).ToListAsync();
        }

        public async Task<IEnumerable<Sales>> GetSalesByClientAsync(int clientId)
        {
            return await _dbSet.Where(s => s.ClientId == clientId).ToListAsync();
        }

        public async Task<IEnumerable<Sales>> GetSalesByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(s => s.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<Sales> GetSaleWithDetailsAsync(int salesId)
        {
            return await _dbSet
                .Include(s => s.Client)
                .Include(s => s.Employee)
                .Include(s => s.SalesDetails)
                    .ThenInclude(sd => sd.PlıntusStock)
                .Include(s => s.SalesDetails)
                    .ThenInclude(sd => sd.InjectionStock)
                .FirstOrDefaultAsync(s => s.Id == salesId);
        }

        public async Task<IEnumerable<Sales>> GetSalesByStatusAsync(string status)
        {
            return await _dbSet.Where(s => s.Status == status).ToListAsync();
        }
    }
} 