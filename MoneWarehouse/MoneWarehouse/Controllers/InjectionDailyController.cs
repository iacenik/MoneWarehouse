using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class InjectionDailyController : Controller
    {
        private readonly IInjectionDailyService _injectionDailyService;

        public InjectionDailyController(IInjectionDailyService injectionDailyService)
        {
            _injectionDailyService = injectionDailyService;
        }

        // GET: InjectionDaily
        public async Task<IActionResult> Index()
        {
            try
            {
                var entries = await _injectionDailyService.GetAllEntriesAsync();
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: InjectionDaily/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var entry = await _injectionDailyService.GetEntryByIdAsync(id);
                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionDaily/Create
        public IActionResult Create()
        {
            // ViewBag veya ViewData ile gerekli veriler (ör. Stok ve Çalışan listeleri) eklenebilir
            return View();
        }

        // POST: InjectionDaily/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InjectionDaily entry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _injectionDailyService.CreateEntryAsync(entry);
                    TempData["SuccessMessage"] = "Günlük üretim kaydı başarıyla oluşturuldu.";
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

        // GET: InjectionDaily/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var entry = await _injectionDailyService.GetEntryByIdAsync(id);
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

        // POST: InjectionDaily/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InjectionDaily entry, int oldQuantity)
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
                    await _injectionDailyService.UpdateEntryAsync(entry, oldQuantity);
                    TempData["SuccessMessage"] = "Günlük üretim kaydı başarıyla güncellendi.";
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

        // GET: InjectionDaily/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entry = await _injectionDailyService.GetEntryByIdAsync(id);
                return View(entry);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InjectionDaily/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _injectionDailyService.DeleteEntryAsync(id);
                TempData["SuccessMessage"] = "Günlük üretim kaydı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionDaily/ByDateRange
        public IActionResult ByDateRange()
        {
            return View();
        }

        // POST: InjectionDaily/ByDateRange
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var entries = await _injectionDailyService.GetEntriesByDateRangeAsync(startDate, endDate);
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

        // GET: InjectionDaily/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            try
            {
                var entries = await _injectionDailyService.GetEntriesByEmployeeAsync(employeeId);
                ViewBag.EmployeeId = employeeId;
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionDaily/ByStock/5
        public async Task<IActionResult> ByStock(int stockId)
        {
            try
            {
                var entries = await _injectionDailyService.GetEntriesByStockIdAsync(stockId);
                ViewBag.StockId = stockId;
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
