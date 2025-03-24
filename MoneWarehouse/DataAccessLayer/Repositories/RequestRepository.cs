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
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        public RequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Request>> GetRequestsByStatusAsync(string status)
        {
            return await _dbSet.Where(r => r.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequestsByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(r => r.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequestsByPriorityAsync(string priority)
        {
            return await _dbSet.Where(r => r.Priority == priority).ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate).ToListAsync();
        }
    }

}