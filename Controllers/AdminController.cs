using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Scoreboard()
        {
            return View();
        }
    }
}
