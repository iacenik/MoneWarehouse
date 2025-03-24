using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IEmployeeService _employeeService;

        public RequestController(IRequestService requestService, IEmployeeService employeeService)
        {
            _requestService = requestService;
            _employeeService = employeeService;
        }

        // GET: Request
        public async Task<IActionResult> Index()
        {
            try
            {
                var requests = await _requestService.GetAllRequestsAsync();
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Request/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var request = await _requestService.GetRequestByIdAsync(id);
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Request/Create
        public async Task<IActionResult> Create()
        {
            // Çalışan listesini ViewBag'e ekle
            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.Employees = employees;
            return View();
        }

        // POST: Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _requestService.CreateRequestAsync(request);
                    TempData["SuccessMessage"] = "Talep başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                // Hata durumunda çalışan listesini tekrar yükle
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Hata durumunda çalışan listesini tekrar yükle
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(request);
            }
        }

        // GET: Request/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var request = await _requestService.GetRequestByIdAsync(id);
                // Çalışan listesini ViewBag'e ekle
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Request/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Request request)
        {
            if (id != request.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _requestService.UpdateRequestAsync(request);
                    TempData["SuccessMessage"] = "Talep başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                // Hata durumunda çalışan listesini tekrar yükle
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Hata durumunda çalışan listesini tekrar yükle
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View(request);
            }
        }

        // GET: Request/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var request = await _requestService.GetRequestByIdAsync(id);
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _requestService.DeleteRequestAsync(id);
                TempData["SuccessMessage"] = "Talep başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Request/ByStatus
        public IActionResult ByStatus()
        {
            // Durum seçeneklerini ViewBag'e ekle
            ViewBag.StatusOptions = new List<string> { "Pending", "In Progress", "Completed", "Cancelled" };
            return View();
        }

        // POST: Request/ByStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByStatus(string status)
        {
            try
            {
                var requests = await _requestService.GetRequestsByStatusAsync(status);
                ViewBag.Status = status;
                return View("StatusResults", requests);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.StatusOptions = new List<string> { "Pending", "In Progress", "Completed", "Cancelled" };
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                ViewBag.StatusOptions = new List<string> { "Pending", "In Progress", "Completed", "Cancelled" };
                return View();
            }
        }

        // GET: Request/ByPriority
        public IActionResult ByPriority()
        {
            // Öncelik seçeneklerini ViewBag'e ekle
            ViewBag.PriorityOptions = new List<string> { "Low", "Normal", "High", "Urgent" };
            return View();
        }

        // POST: Request/ByPriority
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByPriority(string priority)
        {
            try
            {
                var requests = await _requestService.GetRequestsByPriorityAsync(priority);
                ViewBag.Priority = priority;
                return View("PriorityResults", requests);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.PriorityOptions = new List<string> { "Low", "Normal", "High", "Urgent" };
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                ViewBag.PriorityOptions = new List<string> { "Low", "Normal", "High", "Urgent" };
                return View();
            }
        }

        // GET: Request/ByEmployee
        public async Task<IActionResult> ByEmployee()
        {
            // Çalışan listesini ViewBag'e ekle
            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.Employees = employees;
            return View();
        }

        // POST: Request/ByEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            try
            {
                var requests = await _requestService.GetRequestsByEmployeeAsync(employeeId);
                var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
                ViewBag.Employee = employee;
                return View("EmployeeResults", requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.Employees = employees;
                return View();
            }
        }

        // GET: Request/ByDateRange
        public IActionResult ByDateRange()
        {
            return View();
        }

        // POST: Request/ByDateRange
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var requests = await _requestService.GetRequestsByDateRangeAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("DateRangeResults", requests);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View();
            }
        }

        // GET: Request/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var statusCounts = await _requestService.GetRequestStatusCountsAsync();
                ViewBag.StatusCounts = statusCounts;
                var requests = await _requestService.GetAllRequestsAsync();
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Request/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                await _requestService.UpdateRequestStatusAsync(id, status);
                TempData["SuccessMessage"] = "Talep durumu başarıyla güncellendi.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Request/AssignRequest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRequest(int id, string assignedTo)
        {
            try
            {
                await _requestService.AssignRequestAsync(id, assignedTo);
                TempData["SuccessMessage"] = "Talep başarıyla atandı.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}