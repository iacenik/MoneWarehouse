using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ISalesDetailRepository : IGenericRepository<SalesDetail>
    {
        Task<IEnumerable<SalesDetail>> GetDetailsBySalesIdAsync(int salesId);
        Task<IEnumerable<SalesDetail>> GetDetailsByProductTypeAsync(string productType);
        Task<IEnumerable<SalesDetail>> GetDetailsByStockIdAsync(int stockId, string productType);
    }
} 