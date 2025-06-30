using Microsoft.AspNetCore.Mvc;

namespace JustViewer.Controllers;

public class PdfController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<PdfController> _logger;

    public PdfController(IWebHostEnvironment env, ILogger<PdfController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpGet("Pdf/View/{fileName}")]
    public IActionResult View(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("Filename is required");
        }

        fileName = Path.GetFileName(fileName);

        // Check if PDF exists before showing the view
        var filePath = GetPdfFilePath(fileName);
        if (filePath == null)
        {
            return NotFound($"PDF file '{fileName}' not found");
        }

        ViewData["FileName"] = fileName;
        ViewData["FileExists"] = true;
        ViewData["Summary"] = "<p>This document contains important info about XYZ...</p>";

        return View();
    }

    [HttpGet("Pdf/Stream/{fileName}")]
    public IActionResult Stream(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Filename is required");
            }

            // Sanitize filename
            fileName = Path.GetFileName(fileName);

            var filePath = GetPdfFilePath(fileName);
            if (filePath == null)
            {
                _logger.LogError("PDF file not found: {FileName}", fileName);
                return NotFound($"File not found: {fileName}");
            }

            _logger.LogInformation("Serving PDF: {FilePath}", filePath);

            // Set proper headers for PDF streaming
            var contentType = "application/pdf";
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Add CORS headers if needed
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            return File(fileStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming PDF: {FileName}", fileName);
            return StatusCode(500, $"Error loading PDF: {ex.Message}");
        }
    }

    // Helper method to find PDF file
    private string? GetPdfFilePath(string fileName)
    {
        var possiblePaths = new[]
        {
            Path.Combine(_env.WebRootPath, "pdfs", fileName),
            Path.Combine(_env.WebRootPath, "pdf", fileName),
            Path.Combine(_env.WebRootPath, "Pdfs", fileName),
            Path.Combine(_env.WebRootPath, "PDF", fileName),
            Path.Combine(_env.WebRootPath, "files", fileName),
            Path.Combine(_env.WebRootPath, fileName) // Direct in wwwroot
        };

        foreach (var path in possiblePaths)
        {
            if (System.IO.File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    // Endpoint to list available PDFs
    [HttpGet("Pdf/List")]
    public IActionResult List()
    {
        try
        {
            var wwwroot = _env.WebRootPath;
            var pdfFiles = new List<string>();

            if (Directory.Exists(wwwroot))
            {
                var searchPaths = new[]
                {
                    Path.Combine(wwwroot, "pdfs"),
                    Path.Combine(wwwroot, "pdf"),
                    Path.Combine(wwwroot, "Pdfs"),
                    Path.Combine(wwwroot, "PDF"),
                    Path.Combine(wwwroot, "files"),
                    wwwroot
                };

                foreach (var searchPath in searchPaths.Where(Directory.Exists))
                {
                    var files = Directory.GetFiles(searchPath, "*.pdf")
                        .Select(f => Path.GetFileName(f))
                        .Where(f => !pdfFiles.Contains(f, StringComparer.OrdinalIgnoreCase));

                    pdfFiles.AddRange(files);
                }
            }

            return Json(new
            {
                Success = true,
                Files = pdfFiles.OrderBy(f => f).ToArray(),
                Count = pdfFiles.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing PDF files");
            return Json(new { Success = false, Error = ex.Message });
        }
    }

    // Debug endpoint
    [HttpGet("Pdf/Debug")]
    public IActionResult Debug()
    {
        try
        {
            var wwwroot = _env.WebRootPath;
            var info = new
            {
                WebRootPath = wwwroot,
                WebRootExists = Directory.Exists(wwwroot),
                Directories = Directory.Exists(wwwroot)
                    ? Directory.GetDirectories(wwwroot).Select(Path.GetFileName).ToArray()
                    : new string[0],
                AllPdfFiles = Directory.Exists(wwwroot)
                    ? Directory.GetFiles(wwwroot, "*.pdf", SearchOption.AllDirectories)
                        .Select(f => Path.GetRelativePath(wwwroot, f)).ToArray()
                    : new string[0],
                PdfJsExists = System.IO.File.Exists(Path.Combine(wwwroot, "js", "pdfjs", "build", "pdf.min.js")),
                ViewerExists = System.IO.File.Exists(Path.Combine(wwwroot, "js", "pdfjs", "web", "viewer.html"))
            };

            return Json(info);
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }
}