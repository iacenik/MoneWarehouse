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
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.Employees.GetAllAsync();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _unitOfWork.Employees.GetActiveEmployeesAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _unitOfWork.Employees.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            if (string.IsNullOrEmpty(department))
                throw new ArgumentException("Departman bilgisi boş olamaz.");

            return await _unitOfWork.Employees.GetEmployeesByDepartmentAsync(department);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.FirstName))
                throw new ArgumentException("Çalışan adı boş olamaz.");

            if (string.IsNullOrEmpty(employee.LastName))
                throw new ArgumentException("Çalışan soyadı boş olamaz.");

            employee.CreatedDate = DateTime.Now;
            employee.IsActive = true;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(employee.EmployeeId);
            if (existingEmployee == null)
                throw new Exception("Çalışan bulunamadı.");

            if (string.IsNullOrEmpty(employee.FirstName))
                throw new ArgumentException("Çalışan adı boş olamaz.");

            if (string.IsNullOrEmpty(employee.LastName))
                throw new ArgumentException("Çalışan soyadı boş olamaz.");

            // Mevcut çalışan bilgilerini güncelle
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Department = employee.Department;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.HireDate = employee.HireDate;
            existingEmployee.TerminationDate = employee.TerminationDate;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.UpdatedDate = DateTime.Now;

            await _unitOfWork.Employees.UpdateAsync(existingEmployee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new Exception("Çalışan bulunamadı.");

            // Çalışanın üretim kayıtlarını kontrol et
            var injectionDailies = await _unitOfWork.InjectionDailies.GetEntriesByEmployeeAsync(id);
            if (injectionDailies.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu çalışana ait enjeksiyon üretim kayıtları var. Çalışan silinemiyor.");

            var plintusDailies = await _unitOfWork.PlintusDailies.GetEntriesByEmployeeAsync(id);
            if (plintusDailies.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu çalışana ait plintus üretim kayıtları var. Çalışan silinemiyor.");

            // Çalışanın satışlarını kontrol et
            var sales = await _unitOfWork.Sales.GetSalesByEmployeeAsync(id);
            if (sales.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu çalışana ait satış kayıtları var. Çalışan silinemiyor.");

            // Çalışanın taleplerini kontrol et
            var requests = await _unitOfWork.Requests.GetRequestsByEmployeeAsync(id);
            if (requests.GetEnumerator().MoveNext())
                throw new InvalidOperationException("Bu çalışana ait talep kayıtları var. Çalışan silinemiyor.");

            await _unitOfWork.Employees.RemoveAsync(employee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeactivateEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new Exception("Çalışan bulunamadı.");

            employee.IsActive = false;
            employee.TerminationDate = DateTime.Now;
            employee.UpdatedDate = DateTime.Now;

            await _unitOfWork.Employees.UpdateAsync(employee);
            await _unitOfWork.CompleteAsync();
        }
    }
}

