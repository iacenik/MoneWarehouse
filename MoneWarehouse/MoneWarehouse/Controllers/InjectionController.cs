using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class InjectionController : Controller
    {
        private readonly IInjectionService _injectionService;

        public InjectionController(IInjectionService injectionService)
        {
            _injectionService = injectionService;
        }

        // GET: Injection
        public async Task<IActionResult> Index()
        {
            try
            {
                var injections = await _injectionService.GetAllInjectionsAsync();
                return View(injections);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Injection/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var injection = await _injectionService.GetInjectionWithDailyEntriesAsync(id);
                if (injection == null)
                {
                    TempData["ErrorMessage"] = $"Enjeksiyon ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(injection);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Injection/Create
        public IActionResult Create()
        {
            // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
            return View();
        }

        // POST: Injection/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Injection injection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _injectionService.CreateInjectionAsync(injection);
                    TempData["SuccessMessage"] = "Enjeksiyon başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(injection);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(injection);
            }
        }

        // GET: Injection/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var injection = await _injectionService.GetInjectionByIdAsync(id);
                if (injection == null)
                {
                    TempData["ErrorMessage"] = $"Enjeksiyon ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(injection);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Injection/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Injection injection)
        {
            if (id != injection.InjectionId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _injectionService.UpdateInjectionAsync(injection);
                    TempData["SuccessMessage"] = "Enjeksiyon başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(injection);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Size ve Code listeleri) eklenebilir
                return View(injection);
            }
        }

        // GET: Injection/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var injection = await _injectionService.GetInjectionByIdAsync(id);
                if (injection == null)
                {
                    TempData["ErrorMessage"] = $"Enjeksiyon ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(injection);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Injection/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _injectionService.DeleteInjectionAsync(id);
                TempData["SuccessMessage"] = "Enjeksiyon başarıyla silindi.";
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

        // GET: Injection/BySize/5
        public async Task<IActionResult> BySize(int sizeId)
        {
            try
            {
                var injections = await _injectionService.GetInjectionsBySizeAsync(sizeId);
                ViewBag.SizeId = sizeId;
                return View(injections);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Injection/ByCode/5
        public async Task<IActionResult> ByCode(int codeId)
        {
            try
            {
                var injections = await _injectionService.GetInjectionsByCodeAsync(codeId);
                ViewBag.CodeId = codeId;
                return View(injections);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
