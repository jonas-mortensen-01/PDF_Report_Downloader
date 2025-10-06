namespace PDF_Report_Downloader.Models
{
    public class Report
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? PdfUrl { get; set; } = string.Empty;
        public bool? PdfDownloaded { get; set; }
        public string? Status { get; set; } = string.Empty;
    }
}