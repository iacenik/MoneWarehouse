using BusinessLayer.Services;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MoneWarehouse.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            try
            {
                var clients = await _clientService.GetAllClientsAsync();
                return View(clients);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteriler listelenirken bir hata oluştu: {ex.Message}";
                return View(new List<Client>());
            }
        }

        // GET: Client/Active
        public async Task<IActionResult> Active()
        {
            try
            {
                var clients = await _clientService.GetActiveClientsAsync();
                return View("Index", clients);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Aktif müşteriler listelenirken bir hata oluştu: {ex.Message}";
                return View("Index", new List<Client>());
            }
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri detayları görüntülenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Sales/5
        public async Task<IActionResult> Sales(int id)
        {
            try
            {
                var client = await _clientService.GetClientWithSalesAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri satışları görüntülenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/ClientsByCountry
        public async Task<IActionResult> ClientsByCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return View();
            }

            try
            {
                var clients = await _clientService.GetClientsByCountryAsync(country);
                return View("Index", clients);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ülkeye göre müşteriler listelenirken bir hata oluştu: {ex.Message}";
                return View("Index", new List<Client>());
            }
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyName,ContactName,ContactTitle,Address,Phone,Email,RegComNumber,CIFNumber,BankName,IBAN,DeliveryConditions,Country")] Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _clientService.CreateClientAsync(client);
                    TempData["SuccessMessage"] = "Müşteri başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Müşteri oluşturulurken bir hata oluştu: {ex.Message}");
                }
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,CompanyName,ContactName,ContactTitle,Address,Phone,Email,RegComNumber,CIFNumber,BankName,IBAN,DeliveryConditions,Country,IsActive,CreatedDate")] Client client)
        {
            if (id != client.ClientId)
            {
                TempData["ErrorMessage"] = "Geçersiz müşteri ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _clientService.UpdateClientAsync(client);
                    TempData["SuccessMessage"] = "Müşteri başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("not found"))
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", $"Müşteri güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                TempData["SuccessMessage"] = "Müşteri başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = $"Müşteri silinirken bir hata oluştu: {ex.Message}";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Deactivate/5
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = $"ID: {id} olan müşteri bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri bilgileri alınırken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            try
            {
                await _clientService.DeactivateClientAsync(id);
                TempData["SuccessMessage"] = "Müşteri başarıyla deaktif edildi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Müşteri deaktif edilirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
