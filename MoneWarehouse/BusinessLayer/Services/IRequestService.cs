using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<Request>> GetAllRequestsAsync();
        Task<Request> GetRequestByIdAsync(int id);
        Task<IEnumerable<Request>> GetRequestsByStatusAsync(string status);
        Task<IEnumerable<Request>> GetRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<Request>> GetRequestsByPriorityAsync(string priority);
        Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task CreateRequestAsync(Request request);
        Task UpdateRequestAsync(Request request);
        Task DeleteRequestAsync(int id);
        Task UpdateRequestStatusAsync(int id, string status);
        Task AssignRequestAsync(int id, string assignedTo);
        Task<Dictionary<string, int>> GetRequestStatusCountsAsync();
    }
}
