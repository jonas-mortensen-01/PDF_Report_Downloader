namespace PDF_Report_Downloader.Models
{
    public class PDFValidation
    {
        public bool isValidPdf { get; set; }
        public byte[]? pdfBytes { get; set; }
        public string? errorMessage { get; set; }

        public PDFValidation(bool IsValidPdf, byte[]? PdfBytes, string? ErrorMessage)
        {
            isValidPdf = IsValidPdf;
            pdfBytes = PdfBytes;
            errorMessage = ErrorMessage;
        }
    }
}