using Microsoft.AspNetCore.Mvc;

public class EbooksController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EbookDetails()
    {
        return View();
    }
    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return NotFound();

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

        if (!System.IO.File.Exists(path))
            return NotFound();

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

        return File(stream, "application/pdf", fileName);
    }
}
