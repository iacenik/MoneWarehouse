using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class SalesDetailController : Controller
    {
        private readonly ISalesDetailService _salesDetailService;

        public SalesDetailController(ISalesDetailService salesDetailService)
        {
            _salesDetailService = salesDetailService;
        }

        // GET: SalesDetail
        public async Task<IActionResult> Index()
        {
            try
            {
                var details = await _salesDetailService.GetAllDetailsAsync();
                return View(details);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("Error");
            }
        }

        // GET: SalesDetail/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var detail = await _salesDetailService.GetDetailByIdAsync(id);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SalesDetail/Create
        public IActionResult Create(int salesId)
        {
            ViewBag.SalesId = salesId;
            // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
            return View();
        }

        // POST: SalesDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesDetail detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _salesDetailService.CreateDetailAsync(detail);
                    TempData["SuccessMessage"] = "Satış detayı başarıyla oluşturuldu.";
                    return RedirectToAction("Details", "Sales", new { id = detail.SalesId });
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
                ViewBag.SalesId = detail.SalesId;
                return View(detail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
                ViewBag.SalesId = detail.SalesId;
                return View(detail);
            }
        }

        // GET: SalesDetail/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detail = await _salesDetailService.GetDetailByIdAsync(id);
                ViewBag.OldQuantity = detail.Quantity;

                // Ürün tipine göre ID'yi sakla
                if (detail.ProductType == "Plıntus")
                    ViewBag.OldProductId = detail.PlıntusStockId;
                else if (detail.ProductType == "Injection")
                    ViewBag.OldProductId = detail.InjectionStockId;

                ViewBag.OldProductType = detail.ProductType;

                // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SalesDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesDetail detail, int oldQuantity, int? oldProductId, string oldProductType)
        {
            if (id != detail.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _salesDetailService.UpdateDetailAsync(detail, oldQuantity, oldProductId, oldProductType);
                    TempData["SuccessMessage"] = "Satış detayı başarıyla güncellendi.";
                    return RedirectToAction("Details", "Sales", new { id = detail.SalesId });
                }
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
                return View(detail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // ViewBag veya ViewData ile gerekli veriler (ör. Stok listeleri) eklenebilir
                return View(detail);
            }
        }

        // GET: SalesDetail/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var detail = await _salesDetailService.GetDetailByIdAsync(id);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SalesDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var detail = await _salesDetailService.GetDetailByIdAsync(id);
                int salesId = detail.SalesId;

                await _salesDetailService.DeleteDetailAsync(id);
                TempData["SuccessMessage"] = "Satış detayı başarıyla silindi.";
                return RedirectToAction("Details", "Sales", new { id = salesId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SalesDetail/BySales/5
        public async Task<IActionResult> BySales(int salesId)
        {
            try
            {
                var details = await _salesDetailService.GetDetailsBySalesIdAsync(salesId);
                ViewBag.SalesId = salesId;
                return View(details);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SalesDetail/ByProductType
        public IActionResult ByProductType()
        {
            return View();
        }

        // POST: SalesDetail/ByProductType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ByProductType(string productType)
        {
            try
            {
                var details = await _salesDetailService.GetDetailsByProductTypeAsync(productType);
                ViewBag.ProductType = productType;
                return View("ProductTypeResults", details);
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

        // GET: SalesDetail/ProductSalesReport
        public IActionResult ProductSalesReport()
        {
            return View();
        }

        // POST: SalesDetail/ProductSalesCount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductSalesCount(DateTime startDate, DateTime endDate)
        {
            try
            {
                var productSalesCount = await _salesDetailService.GetProductSalesCountAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("ProductSalesCountResults", productSalesCount);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("ProductSalesReport");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("ProductSalesReport");
            }
        }

        // POST: SalesDetail/ProductSalesAmount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductSalesAmount(DateTime startDate, DateTime endDate)
        {
            try
            {
                var productSalesAmount = await _salesDetailService.GetProductSalesAmountAsync(startDate, endDate);
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("ProductSalesAmountResults", productSalesAmount);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("ProductSalesReport");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View("ProductSalesReport");
            }
        }
    }
}
