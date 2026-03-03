using System.Diagnostics;
using BookShelf.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
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
        public IActionResult Category()
        {
            return View();
        }
        public IActionResult BookDetails()
        {
            return View();
        }
        public IActionResult UploaderInfo()
        {
            return View();
        }
        public IActionResult Ebooks()
        {
            return View();
        }
        public IActionResult EbookDetails()
        {
            return View();
        }
        public IActionResult CategoryBooks(string category)
        {
            ViewBag.Category = category ?? "Books";
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
    }
}
