using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class InjectionStockController : Controller
    {
        private readonly IInjectionStockService _injectionStockService;

        public InjectionStockController(IInjectionStockService injectionStockService)
        {
            _injectionStockService = injectionStockService;
        }

        // GET: InjectionStock
        public async Task<IActionResult> Index()
        {
            try
            {
                var stocks = await _injectionStockService.GetAllStocksAsync();
                return View(stocks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: InjectionStock/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var stock = await _injectionStockService.GetStockWithDailyEntriesAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionStock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InjectionStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InjectionStock stock)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _injectionStockService.CreateStockAsync(stock);
                    TempData["SuccessMessage"] = "Stok başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                return View(stock);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(stock);
            }
        }

        // GET: InjectionStock/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var stock = await _injectionStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InjectionStock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InjectionStock stock)
        {
            if (id != stock.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _injectionStockService.UpdateStockAsync(stock);
                    TempData["SuccessMessage"] = "Stok başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                return View(stock);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(stock);
            }
        }

        // GET: InjectionStock/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var stock = await _injectionStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InjectionStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _injectionStockService.DeleteStockAsync(id);
                TempData["SuccessMessage"] = "Stok başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionStock/ByCode
        public IActionResult ByCode()
        {
            return View();
        }

        // POST: InjectionStock/ByCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByCode(string code)
        {
            try
            {
                var stocks = await _injectionStockService.GetStocksByCodeAsync(code);
                ViewBag.Code = code;
                return View("CodeResults", stocks);
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

        // GET: InjectionStock/ManageStock
        public async Task<IActionResult> ManageStock(int id)
        {
            try
            {
                var stock = await _injectionStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InjectionStock/IncreaseStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseStock(int stockId, int quantity)
        {
            try
            {
                await _injectionStockService.IncreaseStockQuantityAsync(stockId, quantity);
                TempData["SuccessMessage"] = "Stok miktarı başarıyla artırıldı.";
                return RedirectToAction(nameof(Details), new { id = stockId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(ManageStock), new { id = stockId });
            }
        }

        // POST: InjectionStock/DecreaseStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseStock(int stockId, int quantity)
        {
            try
            {
                await _injectionStockService.DecreaseStockQuantityAsync(stockId, quantity);
                TempData["SuccessMessage"] = "Stok miktarı başarıyla azaltıldı.";
                return RedirectToAction(nameof(Details), new { id = stockId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(ManageStock), new { id = stockId });
            }
        }

        // GET: InjectionStock/LowStock
        public async Task<IActionResult> LowStock(int threshold = 10)
        {
            try
            {
                var lowStocks = await _injectionStockService.GetLowStockItemsAsync(threshold);
                ViewBag.Threshold = threshold;
                return View(lowStocks);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: InjectionStock/TotalStock
        public async Task<IActionResult> TotalStock()
        {
            try
            {
                int totalQuantity = await _injectionStockService.GetTotalStockQuantityAsync();
                ViewBag.TotalQuantity = totalQuantity;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
