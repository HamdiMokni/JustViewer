const frame = document.getElementById('pdfFrame');
function withViewer(fn) {
    if (!frame.contentWindow || !frame.contentWindow.PDFViewerApplication) return;
    return fn(frame.contentWindow.PDFViewerApplication);
}
function updateStatus(app) {
    document.getElementById('zoomDisplay').textContent = Math.round(app.pdfViewer.currentScale * 100) + '%';
    document.getElementById('pageDisplay').textContent = `Page ${app.page} of ${app.pagesCount}`;
}
frame.addEventListener('load', () => {
    const app = frame.contentWindow.PDFViewerApplication;
    if (app.initialized) updateStatus(app);
    else frame.contentWindow.addEventListener('webviewerloaded', () => updateStatus(frame.contentWindow.PDFViewerApplication));
});
function zoomIn(){ withViewer(app => { app.zoomIn(); updateStatus(app); }); }
function zoomOut(){ withViewer(app => { app.zoomOut(); updateStatus(app); }); }
function nextPage(){ withViewer(app => { app.page = Math.min(app.page + 1, app.pagesCount); updateStatus(app); }); }
function prevPage(){ withViewer(app => { app.page = Math.max(app.page - 1, 1); updateStatus(app); }); }
function printPDF(){ withViewer(app => app.print()); }
function toggleFullscreen(){
    const el = document.documentElement;
    if(!document.fullscreenElement) el.requestFullscreen();
    else document.exitFullscreen();
}
