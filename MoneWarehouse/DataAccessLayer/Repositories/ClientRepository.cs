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
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync()
        {
            return await _dbSet.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetClientsByCountryAsync(string country)
        {
            return await _dbSet.Where(c => c.Country == country).ToListAsync();
        }

        public async Task<Client> GetClientWithSalesAsync(int clientId)
        {
            return await _dbSet
                .Include(c => c.Sales)
                .ThenInclude(s => s.SalesDetails)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);
        }
    }
} 