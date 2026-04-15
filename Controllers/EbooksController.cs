using BookShelf.Data;
using BookShelf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class EbooksController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EbooksController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ✅ LIST ALL EBOOKS
    public async Task<IActionResult> Index()
    {
        var ebooks = await _db.Ebooks
            .Include(e => e.Category)
            .Include(e => e.Uploader)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return View(ebooks);
    }

    // ✅ DETAILS PAGE
    public IActionResult EbookDetails(int id)
    {
        var ebook = _db.Ebooks
            .Include(e => e.Uploader)
            .FirstOrDefault(e => e.Id == id);

        if (ebook == null)
            return NotFound();

        return View(ebook);
    }

    public IActionResult UploadPdf()
    {
        var categories = _db.Categories.ToList(); // DEBUG
        ViewBag.Categories = categories;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> UploadPdf(Ebook model, IFormFile pdfFile)
    {
        // ✅ IMPORTANT: Validate model first
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _db.Categories.ToList();
            return View(model);
        }

        // ✅ File upload
        if (pdfFile != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            model.PdfFilePath = fileName;
        }

        // ✅ Set uploader
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
            model.UploaderId = user.Id;

        _db.Ebooks.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public IActionResult Download(int id)
    {
        var ebook = _db.Ebooks.FirstOrDefault(e => e.Id == id);

        if (ebook == null)
            return NotFound();

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", ebook.PdfFilePath);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var bytes = System.IO.File.ReadAllBytes(filePath);

        // ✅ THIS LINE: download with ORIGINAL NAME (Title)
        var downloadName = ebook.Title + ".pdf";

        return File(bytes, "application/pdf", downloadName);
    }
}