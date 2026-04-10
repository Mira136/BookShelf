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
            var books = _db.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.Id)
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var wishlistBookIds = _db.WishlistItems
                    .Where(w => w.UserId == userId)
                    .Select(w => w.BookId)
                    .ToList();

                foreach (var book in books)
                {
                    book.IsInWishlist = wishlistBookIds.Contains(book.Id);
                }
            }

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
                    .ThenInclude(r => r.User)
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
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var books = _db.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == id)
                .OrderByDescending(b => b.Id) // latest first
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var wishlistBookIds = _db.WishlistItems
                    .Where(w => w.UserId == userId)
                    .Select(w => w.BookId)
                    .ToList();

                foreach (var book in books)
                {
                    book.IsInWishlist = wishlistBookIds.Contains(book.Id);
                }
            }

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
        [HttpPost]
        public async Task<IActionResult> AddReview(int BookId, string Comment, int Rating)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var review = new Review
            {
                BookId = BookId,
                Comment = Comment,
                Rating = Rating,
                UserId = userId
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // 🔥 Update average rating
            var book = await _db.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == BookId);

            if (book != null && book.Reviews.Any())
            {
                book.Rating = book.Reviews.Average(r => r.Rating);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("BookDetails", new { id = BookId });
        }
    }
}
