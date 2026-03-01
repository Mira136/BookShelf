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

    public IActionResult UploadPdf()
    {
        return View();
    }
    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return NotFound();

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

        if (!System.IO.File.Exists(path))
            return NotFound();

        return PhysicalFile(path, "application/pdf", fileName);
    }
}