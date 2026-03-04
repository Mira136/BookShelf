using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
