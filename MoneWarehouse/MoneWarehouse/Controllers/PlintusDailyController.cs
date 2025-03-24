using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class PlintusDailyController : Controller
    {
        private readonly IPlıntusDailyService _plintusDailyService;

        public PlintusDailyController(IPlıntusDailyService plintusDailyService)
        {
            _plintusDailyService = plintusDailyService;
        }

        // GET: PlintusDaily
        public async Task<IActionResult> Index()
        {
            try
            {
                var entries = await _plintusDailyService.GetAllEntriesAsync();
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: PlintusDaily/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var entry = await _plintusDailyService.GetEntryByIdAsync(id);
                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlintusDaily/Create
        public IActionResult Create()
        {
            // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
            return View();
        }

        // POST: PlintusDaily/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlıntusDaily entry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _plintusDailyService.CreateEntryAsync(entry);
                    TempData["SuccessMessage"] = "Günlük plıntus üretim kaydı başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
                return View(entry);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
                return View(entry);
            }
        }

        // GET: PlintusDaily/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var entry = await _plintusDailyService.GetEntryByIdAsync(id);
                ViewBag.OldQuantity = entry.Quantity; // Bu bilgiyi update işleminde kullanacağız
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlintusDaily/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlıntusDaily entry, int oldQuantity)
        {
            if (id != entry.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _plintusDailyService.UpdateEntryAsync(entry, oldQuantity);
                    TempData["SuccessMessage"] = "Günlük plıntus üretim kaydı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
                return View(entry);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
                return View(entry);
            }
        }

        // GET: PlintusDaily/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entry = await _plintusDailyService.GetEntryByIdAsync(id);
                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlintusDaily/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _plintusDailyService.DeleteEntryAsync(id);
                TempData["SuccessMessage"] = "Günlük plıntus üretim kaydı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlintusDaily/ByDateRange
        public IActionResult ByDateRange()
        {
            return View();
        }

        // POST: PlintusDaily/ByDateRange
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var entries = await _plintusDailyService.GetEntriesByDateRangeAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("DateRangeResults", entries);
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

        // GET: PlintusDaily/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            try
            {
                var entries = await _plintusDailyService.GetEntriesByEmployeeAsync(employeeId);
                ViewBag.EmployeeId = employeeId;
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlintusDaily/ByStock/5
        public async Task<IActionResult> ByStock(int stockId)
        {
            try
            {
                var entries = await _plintusDailyService.GetEntriesByStockIdAsync(stockId);
                ViewBag.StockId = stockId;
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlintusDaily/TotalProduction
        public IActionResult TotalProduction()
        {
            return View();
        }

        // POST: PlintusDaily/TotalProduction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TotalProduction(DateTime startDate, DateTime endDate)
        {
            try
            {
                int totalProduction = await _plintusDailyService.GetTotalProductionByDateRangeAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.TotalProduction = totalProduction;
                return View("TotalProductionResults");
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

        // GET: PlintusDaily/ProductionByEmployee
        public IActionResult ProductionByEmployee()
        {
            return View();
        }

        // POST: PlintusDaily/ProductionByEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductionByEmployee(DateTime startDate, DateTime endDate)
        {
            try
            {
                var productionByEmployee = await _plintusDailyService.GetProductionByEmployeeAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("ProductionByEmployeeResults", productionByEmployee);
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
    }
}
