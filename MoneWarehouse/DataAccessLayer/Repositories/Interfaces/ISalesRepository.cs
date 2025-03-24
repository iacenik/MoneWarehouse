using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ISalesRepository : IGenericRepository<Sales>
    {
        Task<IEnumerable<Sales>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sales>> GetSalesByClientAsync(int clientId);
        Task<IEnumerable<Sales>> GetSalesByEmployeeAsync(int employeeId);
        Task<Sales> GetSaleWithDetailsAsync(int salesId);
        Task<IEnumerable<Sales>> GetSalesByStatusAsync(string status);
    }
} 