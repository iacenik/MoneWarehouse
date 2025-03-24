using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class FormulaController : Controller
    {
        private readonly IFormulaService _formulaService;

        public FormulaController(IFormulaService formulaService)
        {
            _formulaService = formulaService;
        }

        // GET: Formula
        public async Task<IActionResult> Index()
        {
            try
            {
                var formulas = await _formulaService.GetAllFormulasAsync();
                return View(formulas);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Formula/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var formula = await _formulaService.GetFormulaByIdAsync(id);
                return View(formula);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Formula/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Formula/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Formula formula)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _formulaService.CreateFormulaAsync(formula);
                    TempData["SuccessMessage"] = "Formül başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                return View(formula);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(formula);
            }
        }

        // GET: Formula/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var formula = await _formulaService.GetFormulaByIdAsync(id);
                return View(formula);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Formula/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Formula formula)
        {
            if (id != formula.FormulaId)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _formulaService.UpdateFormulaAsync(formula);
                    TempData["SuccessMessage"] = "Formül başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                return View(formula);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(formula);
            }
        }

        // GET: Formula/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var formula = await _formulaService.GetFormulaByIdAsync(id);
                return View(formula);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Formula/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _formulaService.DeleteFormulaAsync(id);
                TempData["SuccessMessage"] = "Formül başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Formula/FindByName
        public IActionResult FindByName()
        {
            return View();
        }

        // POST: Formula/FindByName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    ModelState.AddModelError("", "Formül adı boş olamaz.");
                    return View();
                }

                var formula = await _formulaService.GetFormulaByNameAsync(name);
                return RedirectToAction(nameof(Details), new { id = formula.FormulaId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
