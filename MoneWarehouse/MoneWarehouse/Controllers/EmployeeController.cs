using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışanlar listelenirken bir hata oluştu: {ex.Message}";
                return View(new List<Employee>());
            }
        }

        // GET: Employee/Active
        public async Task<IActionResult> Active()
        {
            try
            {
                var employees = await _employeeService.GetActiveEmployeesAsync();
                return View("Index", employees);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Aktif çalışanlar listelenirken bir hata oluştu: {ex.Message}";
                return View("Index", new List<Employee>());
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan çalışan bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan detayları görüntülenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/ByDepartment
        public async Task<IActionResult> ByDepartment(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return View();
            }

            try
            {
                var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
                return View("Index", employees);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Departmana göre çalışanlar listelenirken bir hata oluştu: {ex.Message}";
                return View("Index", new List<Employee>());
            }
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Department,Email,Phone,HireDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.CreateEmployeeAsync(employee);
                    TempData["SuccessMessage"] = "Çalışan başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Çalışan oluşturulurken bir hata oluştu: {ex.Message}");
                }
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan çalışan bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Department,Email,Phone,HireDate,TerminationDate,IsActive,CreatedDate")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                TempData["ErrorMessage"] = "Geçersiz çalışan ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                    TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("not found"))
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", $"Çalışan güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan çalışan bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                TempData["SuccessMessage"] = "Çalışan başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = $"Çalışan silinirken bir hata oluştu: {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/Deactivate/5
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan çalışan bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employee/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            try
            {
                await _employeeService.DeactivateEmployeeAsync(id);
                TempData["SuccessMessage"] = "Çalışan başarıyla deaktif edildi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan deaktif edilirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
