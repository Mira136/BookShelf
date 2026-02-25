using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
