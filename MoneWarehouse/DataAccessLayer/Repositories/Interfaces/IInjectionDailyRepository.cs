using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IInjectionDailyRepository : IGenericRepository<InjectionDaily>
    {
        Task<IEnumerable<InjectionDaily>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InjectionDaily>> GetEntriesByEmployeeAsync(int employeeId);
        Task<IEnumerable<InjectionDaily>> GetEntriesByStockIdAsync(int stockId);
    }
} 