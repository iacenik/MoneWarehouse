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
    public class SalesDetailRepository : GenericRepository<SalesDetail>, ISalesDetailRepository
    {
        public SalesDetailRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<SalesDetail>> GetDetailsBySalesIdAsync(int salesId)
        {
            return await _dbSet.Where(sd => sd.SalesId == salesId).ToListAsync();
        }

        public async Task<IEnumerable<SalesDetail>> GetDetailsByProductTypeAsync(string productType)
        {
            return await _dbSet.Where(sd => sd.ProductType == productType).ToListAsync();
        }

        public async Task<IEnumerable<SalesDetail>> GetDetailsByStockIdAsync(int stockId, string productType)
        {
            if (productType == "Plýntus")
            {
                return await _dbSet.Where(sd => sd.PlýntusStockId == stockId).ToListAsync();
            }
            else if (productType == "Injection")
            {
                return await _dbSet.Where(sd => sd.InjectionStockId == stockId).ToListAsync();
            }

            return new List<SalesDetail>();
        }
    }

}