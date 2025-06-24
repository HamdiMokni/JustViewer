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
        try
        {
            // Sanitize filename to prevent directory traversal
            fileName = Path.GetFileName(fileName);

            var filePath = Path.Combine(_env.WebRootPath, "pdf", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File not found: {fileName}");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error loading PDF: {ex.Message}");
        }
    }
}
