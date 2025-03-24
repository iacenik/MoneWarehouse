using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IRequestRepository : IGenericRepository<Request>
    {
        Task<IEnumerable<Request>> GetRequestsByStatusAsync(string status);
        Task<IEnumerable<Request>> GetRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<Request>> GetRequestsByPriorityAsync(string priority);
        Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 