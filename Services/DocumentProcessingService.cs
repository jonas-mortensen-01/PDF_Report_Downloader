using PDF_Report_Downloader.Data;
using PDF_Report_Downloader.Helpers;
using Microsoft.EntityFrameworkCore;
using PDF_Report_Downloader.Models;

namespace PDF_Report_Downloader.Services
{
    public class DocumentProcessingService
    {
        private readonly ApplicationDbContext _context;

        public DocumentProcessingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessPendingDocuments()
        {
            var documents = _context.Reports
                .Where(r => r.PdfDownloaded == null && r.PdfUrl != null && r.Name != null)
                .Take(10)
                .ToList();

            foreach (var doc in documents)
            {
                var result = !string.IsNullOrEmpty(doc.PdfUrl) ? await ReportHelper.ValidatePDFAsync(doc.PdfUrl) : null;

                if (result != null)
                {
                    doc.Status = result.errorMessage;
                }

                var dto = ReportHelper.ToDto(doc);

                string emailBody = $@"
                    <p><b>Document Name:</b> {dto.Name}</p>
                    <p><b>File Path:</b> {dto.PdfUrl}</p>
                    <p><b>Status:</b> {dto.Status}</p>
                ";

                // EmailHelper.SendEmail("jonas.mortensen@specialisterne.com", "Pending PDF Document", emailBody);

                // Send email
                await EmailHelper.SendEmailWithPdfAsync(
                    smtpHost: "smtp.gmail.com",
                    smtpUser: "jonas.mortensen@specialisterne.com",
                    smtpPass: "babx tbud tvmc ijyt",
                    fromEmail: "jonas.mortensen@specialisterne.com",
                    toEmail: "jonas.mortensen@specialisterne.com",
                    subject: "Report PDF",
                    body: "Please find the report attached.",
                    pdfBytes: result.pdfBytes!,
                    pdfFileName: "Report.pdf");
    
                // Mark as downloaded
                // doc.PdfDownloaded = true;
            }

            _context.SaveChanges();
        }
    }
}
