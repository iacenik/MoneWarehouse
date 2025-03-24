using BusinessLayer.Services;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        // GET: Material
        public async Task<IActionResult> Index()
        {
            try
            {
                var materials = await _materialService.GetAllMaterialsAsync();
                return View(materials);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Material/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialByIdAsync(id);
                return View(material);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Material/Create
        public IActionResult Create()
        {
            // ViewBag veya ViewData ile birim listesi eklenebilir
            ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
            return View();
        }

        // POST: Material/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Material material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _materialService.CreateMaterialAsync(material);
                    TempData["SuccessMessage"] = "Malzeme başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile birim listesi eklenebilir
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View(material);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile birim listesi eklenebilir
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View(material);
            }
        }

        // GET: Material/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialByIdAsync(id);
                // ViewBag veya ViewData ile birim listesi eklenebilir
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View(material);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Material/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Material material)
        {
            if (id != material.MaterialId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _materialService.UpdateMaterialAsync(material);
                    TempData["SuccessMessage"] = "Malzeme başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                // ViewBag veya ViewData ile birim listesi eklenebilir
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View(material);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile birim listesi eklenebilir
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View(material);
            }
        }

        // GET: Material/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialByIdAsync(id);
                return View(material);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _materialService.DeleteMaterialAsync(id);
                TempData["SuccessMessage"] = "Malzeme başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Material/ByUnit
        public IActionResult ByUnit()
        {
            ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
            return View();
        }

        // POST: Material/ByUnit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByUnit(MaterialUnit unit)
        {
            try
            {
                var materials = await _materialService.GetMaterialsByUnitAsync(unit);
                ViewBag.UnitName = unit.ToString();
                return View("UnitResults", materials);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                ViewBag.Units = Enum.GetValues(typeof(MaterialUnit));
                return View();
            }
        }

        // GET: Material/LowQuantity
        public async Task<IActionResult> LowQuantity(double threshold = 10)
        {
            try
            {
                var lowQuantityMaterials = await _materialService.GetLowQuantityMaterialsAsync(threshold);
                ViewBag.Threshold = threshold;
                return View(lowQuantityMaterials);
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
    }
}
