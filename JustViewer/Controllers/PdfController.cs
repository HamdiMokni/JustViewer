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
        ViewData["FileName"] = fileName;
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

            // Debug: Log the web root path
            _logger.LogInformation("WebRootPath: {WebRootPath}", _env.WebRootPath);

            // Try different possible folder names
            var possiblePaths = new[]
            {
                Path.Combine(_env.WebRootPath, "pdfs", fileName),
                Path.Combine(_env.WebRootPath, "pdf", fileName),
                Path.Combine(_env.WebRootPath, "Pdfs", fileName),
                Path.Combine(_env.WebRootPath, "PDF", fileName),
                Path.Combine(_env.WebRootPath, "files", fileName),
                Path.Combine(_env.WebRootPath, fileName) // Direct in wwwroot
            };

            // Debug: Log all paths being checked
            foreach (var path in possiblePaths)
            {
                var exists = System.IO.File.Exists(path);
                _logger.LogInformation("Checking path: {Path} - Exists: {Exists}", path, exists);
            }

            // Debug: List all directories in wwwroot
            try
            {
                var directories = Directory.GetDirectories(_env.WebRootPath);
                _logger.LogInformation("Directories in wwwroot: {Directories}", string.Join(", ", directories.Select(Path.GetFileName)));
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not list directories: {Error}", ex.Message);
            }

            string? filePath = null;
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    filePath = path;
                    _logger.LogInformation("Found PDF at: {FilePath}", filePath);
                    break;
                }
            }

            if (filePath == null)
            {
                _logger.LogError("PDF file not found: {FileName}. Searched {Count} paths", fileName, possiblePaths.Length);

                // Debug: Return detailed error with searched paths
                var searchedPaths = string.Join("\n", possiblePaths);
                return NotFound($"File not found: {fileName}\n\nSearched paths:\n{searchedPaths}");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");
            Response.Headers.Add("Content-Type", "application/pdf");

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading PDF: {FileName}", fileName);
            return StatusCode(500, $"Error loading PDF: {ex.Message}");
        }
    }

    // Debug endpoint to list files
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
                Directories = Directory.Exists(wwwroot) ? Directory.GetDirectories(wwwroot).Select(Path.GetFileName).ToArray() : new string[0],
                AllFiles = Directory.Exists(wwwroot) ? Directory.GetFiles(wwwroot, "*.pdf", SearchOption.AllDirectories).Select(f => Path.GetRelativePath(wwwroot, f)).ToArray() : new string[0]
            };

            return Json(info);
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }
}