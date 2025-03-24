using System.Diagnostics;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using MoneWarehouse.Models;

namespace MoneWarehouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISalesService _salesService;
        private readonly IRequestService _requestService;

        public HomeController(
            ILogger<HomeController> logger,
            ISalesService salesService,
            IRequestService requestService)
        {
            _logger = logger;
            _salesService = salesService;
            _requestService = requestService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
