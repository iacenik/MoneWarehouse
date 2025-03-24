using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class PlintusController : Controller
    {
        private readonly IPlıntusService _plintusService;

        public PlintusController(IPlıntusService plintusService)
        {
            _plintusService = plintusService;
        }

        // GET: Plintus
        public async Task<IActionResult> Index()
        {
            try
            {
                var plintuses = await _plintusService.GetAllPlintusesAsync();
                return View(plintuses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Plintus/Active
        public async Task<IActionResult> Active()
        {
            try
            {
                var activePlintuses = await _plintusService.GetActivePlintusesAsync();
                return View(activePlintuses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Plintus/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var plintus = await _plintusService.GetPlintusByIdAsync(id);
                return View(plintus);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Plintus/Create
        public IActionResult Create()
        {
            // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
            return View();
        }

        // POST: Plintus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plıntus plintus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _plintusService.CreatePlintusAsync(plintus);
                    TempData["SuccessMessage"] = "Plıntus başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(plintus);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(plintus);
            }
        }

        // GET: Plintus/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var plintus = await _plintusService.GetPlintusByIdAsync(id);
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(plintus);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Plintus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plıntus plintus)
        {
            if (id != plintus.PlıntusId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _plintusService.UpdatePlintusAsync(plintus);
                    TempData["SuccessMessage"] = "Plıntus başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(plintus);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(plintus);
            }
        }

        // GET: Plintus/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var plintus = await _plintusService.GetPlintusByIdAsync(id);
                return View(plintus);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Plintus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _plintusService.DeletePlintusAsync(id);
                TempData["SuccessMessage"] = "Plıntus başarıyla silindi.";
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

        // POST: Plintus/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _plintusService.DeactivatePlintusAsync(id);
                TempData["SuccessMessage"] = "Plıntus başarıyla deaktif edildi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Plintus/BySize/5
        public async Task<IActionResult> BySize(int sizeId)
        {
            try
            {
                var plintuses = await _plintusService.GetPlintusesBySizeAsync(sizeId);
                ViewBag.SizeId = sizeId;
                return View(plintuses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Plintus/ByCode/5
        public async Task<IActionResult> ByCode(int codeId)
        {
            try
            {
                var plintuses = await _plintusService.GetPlintusesByCodeAsync(codeId);
                ViewBag.CodeId = codeId;
                return View(plintuses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Plintus/WithDailyEntries/5
        public async Task<IActionResult> WithDailyEntries(int id)
        {
            try
            {
                var plintus = await _plintusService.GetPlintusWithDailyEntriesAsync(id);
                return View(plintus);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Plintus/WithStocks/5
        public async Task<IActionResult> WithStocks(int id)
        {
            try
            {
                var plintus = await _plintusService.GetPlintusWithStocksAsync(id);
                return View(plintus);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
