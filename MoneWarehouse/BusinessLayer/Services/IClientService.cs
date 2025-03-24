using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<IEnumerable<Client>> GetActiveClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> GetClientWithSalesAsync(int id);
        Task<IEnumerable<Client>> GetClientsByCountryAsync(string country);
        Task CreateClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
        Task DeactivateClientAsync(int id);
    }
}
