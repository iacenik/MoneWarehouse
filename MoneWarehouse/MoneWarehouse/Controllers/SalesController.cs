using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoneWarehouse.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISalesService _salesService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly IPlıntusStockService _plintusStockService;
        private readonly IInjectionStockService _injectionStockService;

        public SalesController(
            ISalesService salesService,
            IClientService clientService,
            IEmployeeService employeeService,
            IPlıntusStockService plintusStockService,
            IInjectionStockService injectionStockService)
        {
            _salesService = salesService;
            _clientService = clientService;
            _employeeService = employeeService;
            _plintusStockService = plintusStockService;
            _injectionStockService = injectionStockService;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            try
            {
                var sales = await _salesService.GetAllSalesAsync();
                return View(sales);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satışlar listelenirken bir hata oluştu: {ex.Message}";
                return View(new List<Sales>());
            }
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var sale = await _salesService.GetSaleWithDetailsAsync(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(sale);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış detayları görüntülenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Sales/ByDateRange
        public async Task<IActionResult> ByDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return View();
            }

            try
            {
                var sales = await _salesService.GetSalesByDateRangeAsync(startDate.Value, endDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
                return View("Index", sales);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Tarih aralığına göre satışlar listelenirken bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        // GET: Sales/ByClient/5
        public async Task<IActionResult> ByClient(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var sales = await _salesService.GetSalesByClientAsync(id);
                ViewBag.ClientName = client.CompanyName;
                return View("Index", sales);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteriye göre satışlar listelenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Sales/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan çalışan bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var sales = await _salesService.GetSalesByEmployeeAsync(id);
                ViewBag.EmployeeName = $"{employee.FirstName} {employee.LastName}";
                return View("Index", sales);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışana göre satışlar listelenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Sales/ByStatus
        public async Task<IActionResult> ByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return View();
            }

            try
            {
                var sales = await _salesService.GetSalesByStatusAsync(status);
                ViewBag.Status = status;
                return View("Index", sales);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Duruma göre satışlar listelenirken bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var clients = await _clientService.GetActiveClientsAsync();
                var employees = await _employeeService.GetActiveEmployeesAsync();

                ViewBag.Clients = new SelectList(clients, "ClientId", "CompanyName");
                ViewBag.Employees = new SelectList(employees, "EmployeeId", "EmployeeName");

                var model = new Sales
                {
                    SalesDate = DateTime.Now,
                    Status = "Completed",
                    SalesDetails = new List<SalesDetail>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış oluşturma sayfası yüklenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sales sale)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _salesService.CreateSaleAsync(sale);
                    TempData["SuccessMessage"] = "Satış başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Details), new { id = sale.Id });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Satış oluşturulurken bir hata oluştu: {ex.Message}");
            }

            // Hata durumunda dropdown listeleri yeniden doldur
            try
            {
                var clients = await _clientService.GetActiveClientsAsync();
                var employees = await _employeeService.GetActiveEmployeesAsync();

                ViewBag.Clients = new SelectList(clients, "ClientId", "CompanyName", sale.ClientId);
                ViewBag.Employees = new SelectList(employees, "EmployeeId", "EmployeeName", sale.EmployeeId);
            }
            catch (Exception)
            {
                // Dropdown yeniden oluşturulamazsa devam et
            }

            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var sale = await _salesService.GetSaleWithDetailsAsync(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var clients = await _clientService.GetActiveClientsAsync();
                var employees = await _employeeService.GetActiveEmployeesAsync();

                ViewBag.Clients = new SelectList(clients, "ClientId", "CompanyName", sale.ClientId);
                ViewBag.Employees = new SelectList(employees, "EmployeeId", "EmployeeName", sale.EmployeeId);

                return View(sale);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sales sale)
        {
            if (id != sale.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz satış ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _salesService.UpdateSaleAsync(sale);
                    TempData["SuccessMessage"] = "Satış başarıyla güncellendi.";
                    return RedirectToAction(nameof(Details), new { id = sale.Id });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
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
                ModelState.AddModelError("", $"Satış güncellenirken bir hata oluştu: {ex.Message}");
            }

            // Hata durumunda dropdown listeleri yeniden doldur
            try
            {
                var clients = await _clientService.GetActiveClientsAsync();
                var employees = await _employeeService.GetActiveEmployeesAsync();

                ViewBag.Clients = new SelectList(clients, "ClientId", "CompanyName", sale.ClientId);
                ViewBag.Employees = new SelectList(employees, "EmployeeId", "EmployeeName", sale.EmployeeId);
            }
            catch (Exception)
            {
                // Dropdown yeniden oluşturulamazsa devam et
            }

            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var sale = await _salesService.GetSaleWithDetailsAsync(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(sale);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _salesService.DeleteSaleAsync(id);
                TempData["SuccessMessage"] = "Satış başarıyla silindi.";
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
                    TempData["ErrorMessage"] = $"Satış silinirken bir hata oluştu: {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Sales/AddDetail/5
        public async Task<IActionResult> AddDetail(int id)
        {
            try
            {
                var sale = await _salesService.GetSaleByIdAsync(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var plintusStocks = await _plintusStockService.GetAllStocksAsync();
                var injectionStocks = await _injectionStockService.GetAllStocksAsync();

                ViewBag.SaleId = id;
                ViewBag.PlintusStocks = new SelectList(plintusStocks, "Id", "Name");
                ViewBag.InjectionStocks = new SelectList(injectionStocks, "Id", "Name");

                return View(new SalesDetail { SalesId = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış detayı ekleme sayfası yüklenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Sales/AddDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(SalesDetail detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // SalesDetailService üzerinden detay ekle
                    var salesDetailService = HttpContext.RequestServices.GetService(typeof(ISalesDetailService)) as ISalesDetailService;
                    await salesDetailService.CreateDetailAsync(detail);

                    TempData["SuccessMessage"] = "Satış detayı başarıyla eklendi.";
                    return RedirectToAction(nameof(Details), new { id = detail.SalesId });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Satış detayı eklenirken bir hata oluştu: {ex.Message}");
            }

            try
            {
                var plintusStocks = await _plintusStockService.GetAllStocksAsync();
                var injectionStocks = await _injectionStockService.GetAllStocksAsync();

                ViewBag.SaleId = detail.SalesId;
                ViewBag.PlintusStocks = new SelectList(plintusStocks, "Id", "Name", detail.PlıntusStockId);
                ViewBag.InjectionStocks = new SelectList(injectionStocks, "Id", "Name", detail.InjectionStockId);
            }
            catch (Exception)
            {
                // Dropdown yeniden oluşturulamazsa devam et
            }

            return View(detail);
        }

        // GET: Sales/EditDetail/5
        public async Task<IActionResult> EditDetail(int id)
        {
            try
            {
                // SalesDetailService üzerinden detay getir
                var salesDetailService = HttpContext.RequestServices.GetService(typeof(ISalesDetailService)) as ISalesDetailService;
                var detail = await salesDetailService.GetDetailByIdAsync(id);

                if (detail == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış detayı bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var plintusStocks = await _plintusStockService.GetAllStocksAsync();
                var injectionStocks = await _injectionStockService.GetAllStocksAsync();

                ViewBag.PlintusStocks = new SelectList(plintusStocks, "Id", "Name", detail.PlıntusStockId);
                ViewBag.InjectionStocks = new SelectList(injectionStocks, "Id", "Name", detail.InjectionStockId);

                // Orijinal değerleri ViewBag'e aktar (güncelleme için)
                ViewBag.OldQuantity = detail.Quantity;
                ViewBag.OldProductId = detail.ProductType == "Plıntus" ? detail.PlıntusStockId : detail.InjectionStockId;
                ViewBag.OldProductType = detail.ProductType;

                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış detayı bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Sales/EditDetail/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetail(int id, SalesDetail detail, int oldQuantity, int? oldProductId, string oldProductType)
        {
            if (id != detail.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz satış detayı ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // SalesDetailService üzerinden detay güncelle
                    var salesDetailService = HttpContext.RequestServices.GetService(typeof(ISalesDetailService)) as ISalesDetailService;
                    await salesDetailService.UpdateDetailAsync(detail, oldQuantity, oldProductId, oldProductType);

                    TempData["SuccessMessage"] = "Satış detayı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Details), new { id = detail.SalesId });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
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
                ModelState.AddModelError("", $"Satış detayı güncellenirken bir hata oluştu: {ex.Message}");
            }

            try
            {
                var plintusStocks = await _plintusStockService.GetAllStocksAsync();
                var injectionStocks = await _injectionStockService.GetAllStocksAsync();

                ViewBag.PlintusStocks = new SelectList(plintusStocks, "Id", "Name", detail.PlıntusStockId);
                ViewBag.InjectionStocks = new SelectList(injectionStocks, "Id", "Name", detail.InjectionStockId);

                // Orijinal değerleri ViewBag'e aktar (güncelleme için)
                ViewBag.OldQuantity = oldQuantity;
                ViewBag.OldProductId = oldProductId;
                ViewBag.OldProductType = oldProductType;
            }
            catch (Exception)
            {
                // Dropdown yeniden oluşturulamazsa devam et
            }

            return View(detail);
        }

        // GET: Sales/DeleteDetail/5
        public async Task<IActionResult> DeleteDetail(int id)
        {
            try
            {
                // SalesDetailService üzerinden detay getir
                var salesDetailService = HttpContext.RequestServices.GetService(typeof(ISalesDetailService)) as ISalesDetailService;
                var detail = await salesDetailService.GetDetailByIdAsync(id);

                if (detail == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan satış detayı bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Satış detayı bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Sales/DeleteDetail/5
        [HttpPost, ActionName("DeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDetailConfirmed(int id)
        {
            try
            {
                // SalesDetailService üzerinden detay getir (SalesId için)
                var salesDetailService = HttpContext.RequestServices.GetService(typeof(ISalesDetailService)) as ISalesDetailService;
                var detail = await salesDetailService.GetDetailByIdAsync(id);
                int salesId = detail.SalesId;

                // Detayı sil
                await salesDetailService.DeleteDetailAsync(id);

                TempData["SuccessMessage"] = "Satış detayı başarıyla silindi.";
                return RedirectToAction(nameof(Details), new { id = salesId });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = $"Satış detayı silinirken bir hata oluştu: {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Sales/Reports
        public IActionResult Reports()
        {
            return View();
        }

        // GET: Sales/MonthlySalesReport
        public async Task<IActionResult> MonthlySalesReport(int? year)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            try
            {
                var report = await _salesService.GetMonthlySalesReportAsync(year.Value);
                ViewBag.Year = year.Value;
                return View(report);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Aylık satış raporu oluşturulurken bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        // GET: Sales/ClientSalesReport
        public async Task<IActionResult> ClientSalesReport(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = new DateTime(DateTime.Now.Year, 1, 1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            try
            {
                var report = await _salesService.GetSalesByClientReportAsync(startDate.Value, endDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

                // Müşteri adlarını getir
                var clientIds = report.Keys;
                var clientNames = new Dictionary<int, string>();

                foreach (var clientId in clientIds)
                {
                    var client = await _clientService.GetClientByIdAsync(clientId);
                    if (client != null)
                    {
                        clientNames[clientId] = client.CompanyName;
                    }
                }

                ViewBag.ClientNames = clientNames;

                return View(report);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri satış raporu oluşturulurken bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        // GET: Sales/EmployeeSalesReport
        public async Task<IActionResult> EmployeeSalesReport(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = new DateTime(DateTime.Now.Year, 1, 1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            try
            {
                var report = await _salesService.GetSalesByEmployeeReportAsync(startDate.Value, endDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

                // Çalışan adlarını getir
                var employeeIds = report.Keys;
                var employeeNames = new Dictionary<int, string>();

                foreach (var employeeId in employeeIds)
                {
                    var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
                    if (employee != null)
                    {
                        employeeNames[employeeId] = $"{employee.FirstName} {employee.LastName}";
                    }
                }

                ViewBag.EmployeeNames = employeeNames;

                return View(report);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Çalışan satış raporu oluşturulurken bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        // GET: Sales/TotalSalesAmount
        public async Task<IActionResult> TotalSalesAmount(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = new DateTime(DateTime.Now.Year, 1, 1);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            try
            {
                var totalAmount = await _salesService.CalculateTotalSalesAmountByDateRangeAsync(startDate.Value, endDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
                ViewBag.TotalAmount = totalAmount;

                return View();
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Toplam satış tutarı hesaplanırken bir hata oluştu: {ex.Message}";
                return View();
            }
        }
    }
}
