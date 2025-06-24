#!/usr/bin/env bash
# Simple helper to download PDF.js distribution into wwwroot/js/pdfjs
# Requires curl or wget and internet access.
set -e
VERSION=3.11.174
TARGET_DIR="$(dirname "$0")/../JustViewer/wwwroot/js/pdfjs"
mkdir -p "$TARGET_DIR"
BASE_URL="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/$VERSION"

curl -L "$BASE_URL/pdf.min.js" -o "$TARGET_DIR/pdf.min.js"
curl -L "$BASE_URL/pdf.worker.min.js" -o "$TARGET_DIR/pdf.worker.min.js"
curl -L "$BASE_URL/pdf_viewer.min.js" -o "$TARGET_DIR/pdf_viewer.min.js"
curl -L "$BASE_URL/pdf_viewer.min.css" -o "$TARGET_DIR/pdf_viewer.min.css"
# Download the viewer HTML
curl -L "https://raw.githubusercontent.com/mozilla/pdf.js/v$VERSION/web/viewer.html" -o "$TARGET_DIR/viewer.html"
