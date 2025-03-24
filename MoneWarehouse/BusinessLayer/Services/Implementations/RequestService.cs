using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Request>> GetAllRequestsAsync()
        {
            return await _unitOfWork.Requests.GetAllAsync();
        }

        public async Task<Request> GetRequestByIdAsync(int id)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Talep bulunamadı.");

            return request;
        }

        public async Task<IEnumerable<Request>> GetRequestsByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Durum bilgisi boş olamaz.");

            return await _unitOfWork.Requests.GetRequestsByStatusAsync(status);
        }

        public async Task<IEnumerable<Request>> GetRequestsByEmployeeAsync(int employeeId)
        {
            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            return await _unitOfWork.Requests.GetRequestsByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<Request>> GetRequestsByPriorityAsync(string priority)
        {
            if (string.IsNullOrEmpty(priority))
                throw new ArgumentException("Öncelik bilgisi boş olamaz.");

            return await _unitOfWork.Requests.GetRequestsByPriorityAsync(priority);
        }

        public async Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            return await _unitOfWork.Requests.GetRequestsByDateRangeAsync(startDate, endDate);
        }

        public async Task CreateRequestAsync(Request request)
        {
            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            if (string.IsNullOrEmpty(request.RequestNumber))
                throw new ArgumentException("Talep numarası boş olamaz.");

            // Talep numarasının benzersiz olduğunu kontrol et
            var existingRequests = await _unitOfWork.Requests.FindAsync(r => r.RequestNumber == request.RequestNumber);
            if (existingRequests.Any())
                throw new InvalidOperationException("Bu talep numarası zaten kullanılmakta.");

            // Varsayılan değerleri ayarla
            request.CreatedDate = DateTime.Now;
            request.Status = "Pending";
            request.Priority = request.Priority ?? "Normal";
            request.LastUpdated = DateTime.Now;

            await _unitOfWork.Requests.AddAsync(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateRequestAsync(Request request)
        {
            var existingRequest = await _unitOfWork.Requests.GetByIdAsync(request.Id);
            if (existingRequest == null)
                throw new Exception("Talep bulunamadı.");

            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            if (string.IsNullOrEmpty(request.RequestNumber))
                throw new ArgumentException("Talep numarası boş olamaz.");

            // Talep numarası değişiyorsa, yeni numaranın benzersiz olduğunu kontrol et
            if (existingRequest.RequestNumber != request.RequestNumber)
            {
                var existingRequests = await _unitOfWork.Requests.FindAsync(r => r.RequestNumber == request.RequestNumber);
                if (existingRequests.Any())
                    throw new InvalidOperationException("Bu talep numarası zaten kullanılmakta.");
            }

            // Mevcut talep bilgilerini güncelle
            existingRequest.RequestNumber = request.RequestNumber;
            existingRequest.EmployeeId = request.EmployeeId;
            existingRequest.Status = request.Status;
            existingRequest.Priority = request.Priority;
            existingRequest.Description = request.Description;
            existingRequest.RequestType = request.RequestType;
            existingRequest.DueDate = request.DueDate;
            existingRequest.AssignedTo = request.AssignedTo;
            existingRequest.Notes = request.Notes;
            existingRequest.LastUpdated = DateTime.Now;
            existingRequest.UpdatedBy = request.UpdatedBy;

            // Talep tamamlandı olarak işaretlendiyse
            if (request.Status == "Completed" && existingRequest.Status != "Completed")
                existingRequest.CompletedDate = DateTime.Now;
            else if (request.Status != "Completed")
                existingRequest.CompletedDate = null;

            await _unitOfWork.Requests.UpdateAsync(existingRequest);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteRequestAsync(int id)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Talep bulunamadı.");

            await _unitOfWork.Requests.RemoveAsync(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateRequestStatusAsync(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Durum bilgisi boş olamaz.");

            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Talep bulunamadı.");

            request.Status = status;
            request.LastUpdated = DateTime.Now;

            // Talep tamamlandı olarak işaretlendiyse
            if (status == "Completed" && request.Status != "Completed")
                request.CompletedDate = DateTime.Now;
            else if (status != "Completed")
                request.CompletedDate = null;

            await _unitOfWork.Requests.UpdateAsync(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AssignRequestAsync(int id, string assignedTo)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Talep bulunamadı.");

            request.AssignedTo = assignedTo;
            request.LastUpdated = DateTime.Now;

            await _unitOfWork.Requests.UpdateAsync(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<Dictionary<string, int>> GetRequestStatusCountsAsync()
        {
            var requests = await _unitOfWork.Requests.GetAllAsync();

            return requests
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }

}
