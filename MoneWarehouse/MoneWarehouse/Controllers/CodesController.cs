using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class CodesController : Controller
    {
        private readonly ICodesService _codesService;

        public CodesController(ICodesService codesService)
        {
            _codesService = codesService;
        }

        // GET: Codes
        public async Task<IActionResult> Index()
        {
            try
            {
                var codes = await _codesService.GetAllCodesAsync();
                return View(codes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Codes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var code = await _codesService.GetCodeByIdAsync(id);
                if (code == null)
                {
                    TempData["ErrorMessage"] = $"Kod ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Codes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Codes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Codes code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _codesService.CreateCodeAsync(code);
                    TempData["SuccessMessage"] = "Kod başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                return View(code);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View(code);
            }
        }

        // GET: Codes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var code = await _codesService.GetCodeByIdAsync(id);
                if (code == null)
                {
                    TempData["ErrorMessage"] = $"Kod ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Codes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Codes code)
        {
            if (id != code.CodeId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _codesService.UpdateCodeAsync(code);
                    TempData["SuccessMessage"] = "Kod başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                return View(code);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(code);
            }
            catch (Exception ex) when (ex.Message.Contains("Kod bulunamadı"))
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View(code);
            }
        }

        // GET: Codes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var code = await _codesService.GetCodeByIdAsync(id);
                if (code == null)
                {
                    TempData["ErrorMessage"] = $"Kod ID {id} bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Codes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _codesService.DeleteCodeAsync(id);
                TempData["SuccessMessage"] = "Kod başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex.Message.Contains("Kod bulunamadı"))
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

        // GET: Codes/BySize/5
        public async Task<IActionResult> BySize(int sizeId)
        {
            try
            {
                var codes = await _codesService.GetCodesBySizeAsync(sizeId);
                ViewBag.SizeId = sizeId;
                return View(codes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
