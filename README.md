# JustViewer

JustViewer is a minimal ASP.NET Core MVC application demonstrating how to stream and view large PDF files efficiently.
The PDF viewer is now presented in a dedicated full page using custom CSS while still loading the bundled `viewer.html` from PDF.js.

## Requirements

- .NET 9 SDK
- Optional: `curl` (or `wget`) to download PDF.js assets

## Setup

1. **Fetch PDF.js** (first time only)

   Run the helper script to download the PDF.js distribution (including `viewer.html` and associated scripts) into `wwwroot/js/pdfjs/`:

   ```bash
   ./scripts/fetch_pdfjs.sh
   ```

2. **Run the application**

   ```bash
   dotnet run --project JustViewer/JustViewer.csproj
   ```

3. Navigate to `https://localhost:5001` (or the printed URL) and open the sample PDF from the home page.

The sample PDF is located in `wwwroot/pdf/sample.pdf`. Add your own PDF files to this folder and open them via `/Pdf/View/{fileName}`.

## Notes

- PDF streaming endpoint supports HTTP range requests for fast incremental loading of large files.
- The viewer page simply embeds PDF.js' default `viewer.html` so all default controls (zoom, search, bookmarks, thumbnails, print, download) are available.
