class PDFViewer {
    constructor(fileName) {
        this.fileName = fileName;
        this.frame = document.getElementById('pdfFrame');
        this.loading = document.getElementById('loading');
        this.error = document.getElementById('error');

        this.init();
    }

    init() {
        console.log('Initializing PDF viewer for:', this.fileName);
        this.initializePdfViewer();
    }

    initializePdfViewer() {
        const pdfUrl = `/Pdf/Stream/${encodeURIComponent(this.fileName)}`;

        // Local PDF.js viewer path (downloaded from https://github.com/mozilla/pdf.js)
        const viewerUrl = `/js/pdfjs/web/viewer.html?file=${encodeURIComponent(pdfUrl)}`;

        console.log('Loading PDF:', pdfUrl);
        console.log('Viewer URL:', viewerUrl);

        this.frame.onload = () => {
            this.loading.style.display = 'none';
            this.frame.style.display = 'block';
        };

        this.frame.onerror = () => {
            this.loading.style.display = 'none';
            this.error.style.display = 'block';
        };

        this.frame.src = viewerUrl;
    }
}

// Retry logic (for button click)
function retryLoading() {
    document.getElementById('error').style.display = 'none';
    document.getElementById('loading').style.display = 'block';
    document.getElementById('pdfFrame').src = ''; // Reset iframe
    setTimeout(() => {
        new PDFViewer(window.pdfFileName);
    }, 100); // slight delay
}

// Start viewer on page load
document.addEventListener('DOMContentLoaded', () => {
    if (window.pdfFileName) {
        new PDFViewer(window.pdfFileName);
    }
});
