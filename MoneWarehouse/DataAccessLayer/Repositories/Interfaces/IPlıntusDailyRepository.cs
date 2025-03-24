using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IPlıntusDailyRepository : IGenericRepository<PlıntusDaily>
    {
        Task<IEnumerable<PlıntusDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PlıntusDaily>> GetEntriesByEmployeeAsync(int employeeId);
        Task<IEnumerable<PlıntusDaily>> GetEntriesByStockIdAsync(int stockId);
    }
} 