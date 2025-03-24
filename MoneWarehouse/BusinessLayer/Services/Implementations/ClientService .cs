using DataAccessLayer;
using DataAccessLayer.Repositories.Interfaces;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _unitOfWork.Clients.GetAllAsync();
        }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync()
        {
            return await _unitOfWork.Clients.GetActiveClientsAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _unitOfWork.Clients.GetByIdAsync(id);
        }

        public async Task<Client> GetClientWithSalesAsync(int id)
        {
            return await _unitOfWork.Clients.GetClientWithSalesAsync(id);
        }

        public async Task<IEnumerable<Client>> GetClientsByCountryAsync(string country)
        {
            return await _unitOfWork.Clients.GetClientsByCountryAsync(country);
        }

        public async Task CreateClientAsync(Client client)
        {
            if (string.IsNullOrEmpty(client.CompanyName))
                throw new ArgumentException("Şirket adı boş olamaz.");

            if (string.IsNullOrEmpty(client.ContactName))
                throw new ArgumentException("İletişim adı boş olamaz.");

            if (string.IsNullOrEmpty(client.Country))
                throw new ArgumentException("Ülke bilgisi boş olamaz.");

            client.CreatedDate = DateTime.Now;
            client.IsActive = true;

            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateClientAsync(Client client)
        {
            var existingClient = await _unitOfWork.Clients.GetByIdAsync(client.ClientId);
            if (existingClient == null)
                throw new Exception("Müşteri bulunamadı.");

            if (string.IsNullOrEmpty(client.CompanyName))
                throw new ArgumentException("Şirket adı boş olamaz.");

            if (string.IsNullOrEmpty(client.ContactName))
                throw new ArgumentException("İletişim adı boş olamaz.");

            if (string.IsNullOrEmpty(client.Country))
                throw new ArgumentException("Ülke bilgisi boş olamaz.");

            // Mevcut müşteri bilgilerini güncelle
            existingClient.CompanyName = client.CompanyName;
            existingClient.ContactName = client.ContactName;
            existingClient.ContactTitle = client.ContactTitle;
            existingClient.Address = client.Address;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;
            existingClient.RegComNumber = client.RegComNumber;
            existingClient.CIFNumber = client.CIFNumber;
            existingClient.BankName = client.BankName;
            existingClient.IBAN = client.IBAN;
            existingClient.DeliveryConditions = client.DeliveryConditions;
            existingClient.Country = client.Country;
            existingClient.IsActive = client.IsActive;

            await _unitOfWork.Clients.UpdateAsync(existingClient);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
                throw new Exception("Müşteri bulunamadı.");

            // Müşterinin satışlarını kontrol et
            var clientWithSales = await _unitOfWork.Clients.GetClientWithSalesAsync(id);
            if (clientWithSales.Sales.Count > 0)
                throw new InvalidOperationException("Bu müşteriye ait satışlar var. Müşteri silinemiyor.");

            await _unitOfWork.Clients.RemoveAsync(client);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeactivateClientAsync(int id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null)
                throw new Exception("Müşteri bulunamadı.");

            client.IsActive = false;
            await _unitOfWork.Clients.UpdateAsync(client);
            await _unitOfWork.CompleteAsync();
        }
    }
}
