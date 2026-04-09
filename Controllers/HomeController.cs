using System.Diagnostics;
using BookShelf.Models;
using Microsoft.AspNetCore.Mvc;
using BookShelf.Data;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            var books = _db.Books
                .Include(b => b.Category)
                .Where(b => b.IsAvailable)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(books);
        }
        public IActionResult Category()
        {
            var categories = _db.Categories.ToList();
            return View(categories);
        }
        public IActionResult BookDetails(int id)
        {
            var book = _db.Books
                .Include(b => b.Category)
                .Include(b => b.Uploader)
                .Include(b => b.Reviews)
                .FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }
        public IActionResult Ebooks()
        {
            return View();
        }
        public IActionResult EbookDetails()
        {
            return View();
        }
        public IActionResult CategoryBooks(int id)
        {
            var books = _db.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == id && b.IsAvailable)
                .ToList();

            var category = _db.Categories.FirstOrDefault(c => c.Id == id);

            ViewBag.CategoryName = category?.Name;

            return View(books);
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
        public async Task<IActionResult> UploaderInfo(int id)
        {
            var book = await _db.Books
                .Include(b => b.Uploader)
                .FirstOrDefaultAsync(b => b.Id == id);

            return View(book);
        }
    }
}
