using Microsoft.AspNetCore.Mvc;

namespace JustViewer.Controllers;

public class PdfController : Controller
{
    private readonly IWebHostEnvironment _env;

    public PdfController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("Pdf/View/{fileName}")]
    public IActionResult View(string fileName)
    {
        ViewData["FileName"] = fileName;
        return View();
    }

    [HttpGet("Pdf/Stream/{fileName}")]
    public IActionResult Stream(string fileName)
    {
        var filePath = Path.Combine(_env.WebRootPath, "pdf", fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, "application/pdf", enableRangeProcessing: true);
    }
}
