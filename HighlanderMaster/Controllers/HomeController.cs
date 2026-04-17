using HighlanderMaster.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HighlanderMaster.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(string Name, string Email, string Subject, string Message)
        {
            TempData["Success"] = "Съобщението е изпратено успешно! Ще се свържем с теб скоро.";
            return RedirectToAction("Contact");
        }
    }
}