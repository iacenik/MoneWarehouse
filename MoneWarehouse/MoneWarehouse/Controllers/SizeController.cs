using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // GET: Size
        public async Task<IActionResult> Index()
        {
            try
            {
                var sizes = await _sizeService.GetAllSizesAsync();
                return View(sizes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Size/Active
        public async Task<IActionResult> Active()
        {
            try
            {
                var activeSizes = await _sizeService.GetActiveSizesAsync();
                return View(activeSizes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Size/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id);
                return View(size);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Size/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Size/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Size size)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _sizeService.CreateSizeAsync(size);
                    TempData["SuccessMessage"] = "Boyut başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                return View(size);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(size);
            }
        }

        // GET: Size/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id);
                return View(size);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Size/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Size size)
        {
            if (id != size.SizeId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _sizeService.UpdateSizeAsync(size);
                    TempData["SuccessMessage"] = "Boyut başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                return View(size);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(size);
            }
        }

        // GET: Size/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id);
                return View(size);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Size/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _sizeService.DeleteSizeAsync(id);
                TempData["SuccessMessage"] = "Boyut başarıyla silindi.";
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

        // POST: Size/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _sizeService.DeactivateSizeAsync(id);
                TempData["SuccessMessage"] = "Boyut başarıyla deaktif edildi.";
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

        // GET: Size/ByType
        public IActionResult ByType()
        {
            return View();
        }

        // POST: Size/ByType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByType(string sizeType)
        {
            try
            {
                var sizes = await _sizeService.GetSizesByTypeAsync(sizeType);
                ViewBag.SizeType = sizeType;
                return View("TypeResults", sizes);
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

        // GET: Size/WithCodes/5
        public async Task<IActionResult> WithCodes(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeWithCodesAsync(id);
                return View(size);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }

}
