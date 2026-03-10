using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult UploadBook()
        {
            return View();
        }
        public IActionResult UploadedItems()
        {
            return View();
        }
    }
}