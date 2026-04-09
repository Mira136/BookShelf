using BookShelf.Data;
using BookShelf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public BooksController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // ================= UPLOAD BOOK (GET) =================

        [Authorize]
        public IActionResult UploadBook()
        {
            return View();
        }

        // ================= UPLOAD BOOK (POST) =================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadBook(
            string bookTitle,
            string author,
            decimal sellingPrice,
            decimal? actualPrice,
            string category,
            string condition,
            string language,
            string? educationLevel,
            string description,
            string location,
            IFormFile? photoInput)
        {
            // Get logged in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Find or create category
            var cat = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == category.ToLower());

            if (cat == null)
            {
                cat = new Category { Name = category };
                _db.Categories.Add(cat);
                await _db.SaveChangesAsync();
            }

            // Handle image upload
            string? imagePath = null;
            if (photoInput != null && photoInput.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + "_" + photoInput.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoInput.CopyToAsync(stream);
                }
                imagePath = fileName;
            }

            // Create book
            var book = new Book
            {
                Title = bookTitle,
                Author = author,
                SellingPrice = sellingPrice,
                ActualPrice = actualPrice,
                Condition = condition,
                Language = language,
                EducationLevel = educationLevel,
                Description = description,
                Location = location,
                ImagePath = imagePath,
                CategoryId = cat.Id,
                UploaderId = user.Id,
                IsAvailable = true,
                CreatedAt = DateTime.Now
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            return RedirectToAction("UploadedItems");
        }

        // ================= UPLOADED ITEMS =================

        [Authorize]
        public async Task<IActionResult> UploadedItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Get books uploaded by this user
            var books = await _db.Books
                .Include(b => b.Category)
                .Where(b => b.UploaderId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // Get ebooks uploaded by this user
            var ebooks = await _db.Ebooks
                .Include(e => e.Category)
                .Where(e => e.UploaderId == user.Id)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            ViewBag.Books = books;
            ViewBag.Ebooks = ebooks;

            return View();
        }

        // ================= DELETE BOOK =================

        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var book = await _db.Books.FindAsync(id);

            // Only uploader can delete their own book
            if (book != null && book.UploaderId == user?.Id)
            {
                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("UploadedItems");
        }
        // ================= BOOK DETAILS =================

        public async Task<IActionResult> Details(int id)
        {
            var book = await _db.Books
                .Include(b => b.Category)
                .Include(b => b.Uploader)
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View("~/Views/Home/BookDetails.cshtml", book);
        }
    }
}