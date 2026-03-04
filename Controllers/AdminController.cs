using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public IActionResult Books()
        {
            return View();
        }
        public IActionResult Ebooks()
        {
            return View();
        }
    }
}
