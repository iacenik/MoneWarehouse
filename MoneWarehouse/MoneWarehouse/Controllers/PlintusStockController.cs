using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class PlintusStockController : Controller
    {
        private readonly IPlıntusStockService _plintusStockService;

        public PlintusStockController(IPlıntusStockService plintusStockService)
        {
            _plintusStockService = plintusStockService;
        }

        // GET: PlintusStock
        public async Task<IActionResult> Index()
        {
            try
            {
                var stocks = await _plintusStockService.GetAllStocksAsync();
                return View(stocks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: PlintusStock/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var stock = await _plintusStockService.GetStockWithDailyEntriesAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlintusStock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlintusStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlıntusStock stock)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _plintusStockService.CreateStockAsync(stock);
                    TempData["SuccessMessage"] = "Plıntus stoğu başarıyla oluşturuldu.";
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

        // GET: PlintusStock/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var stock = await _plintusStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlintusStock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlıntusStock stock)
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
                    await _plintusStockService.UpdateStockAsync(stock);
                    TempData["SuccessMessage"] = "Plıntus stoğu başarıyla güncellendi.";
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

        // GET: PlintusStock/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var stock = await _plintusStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlintusStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _plintusStockService.DeleteStockAsync(id);
                TempData["SuccessMessage"] = "Plıntus stoğu başarıyla silindi.";
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

        // GET: PlintusStock/ByCode
        public IActionResult ByCode()
        {
            return View();
        }

        // POST: PlintusStock/ByCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByCode(string code)
        {
            try
            {
                var stocks = await _plintusStockService.GetStocksByCodeAsync(code);
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

        // GET: PlintusStock/ManageStock
        public async Task<IActionResult> ManageStock(int id)
        {
            try
            {
                var stock = await _plintusStockService.GetStockByIdAsync(id);
                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlintusStock/IncreaseStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseStock(int stockId, int quantity)
        {
            try
            {
                await _plintusStockService.IncreaseStockQuantityAsync(stockId, quantity);
                TempData["SuccessMessage"] = "Stok miktarı başarıyla artırıldı.";
                return RedirectToAction(nameof(Details), new { id = stockId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(ManageStock), new { id = stockId });
            }
        }

        // POST: PlintusStock/DecreaseStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseStock(int stockId, int quantity)
        {
            try
            {
                await _plintusStockService.DecreaseStockQuantityAsync(stockId, quantity);
                TempData["SuccessMessage"] = "Stok miktarı başarıyla azaltıldı.";
                return RedirectToAction(nameof(Details), new { id = stockId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(ManageStock), new { id = stockId });
            }
        }

        // GET: PlintusStock/LowStock
        public async Task<IActionResult> LowStock(int threshold = 10)
        {
            try
            {
                var lowStocks = await _plintusStockService.GetLowStockItemsAsync(threshold);
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

        // GET: PlintusStock/TotalStock
        public async Task<IActionResult> TotalStock()
        {
            try
            {
                int totalQuantity = await _plintusStockService.GetTotalStockQuantityAsync();
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
