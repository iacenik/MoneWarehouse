using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<IEnumerable<Client>> GetActiveClientsAsync();
        Task<IEnumerable<Client>> GetClientsByCountryAsync(string country);
        Task<Client> GetClientWithSalesAsync(int clientId);
    }

}