using PDF_Report_Downloader.Data;
using PDF_Report_Downloader.Helpers;
using PDF_Report_Downloader.Controllers;
using Microsoft.EntityFrameworkCore;
using PDF_Report_Downloader.Models;
using Microsoft.Extensions.Configuration;

namespace PDF_Report_Downloader.Services
{
    public class DocumentProcessingService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public DocumentProcessingService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task ProcessPendingDocuments()
        {
            var documents = _context.Reports
                .Where(r => r.PdfUrl != null && r.Name != null && r.Name == "William Tell")
                .Take(1)
                .ToList();

            var processingTasks = documents.Select(async doc =>
            {
                try
                {
                    var result = !string.IsNullOrEmpty(doc.PdfUrl)
                        ? await ReportController.ValidatePDFAsync(doc.PdfUrl)
                        : null;

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

                    if (result != null && !result.isValidPdf)
                    {
                        emailBody = "<p>Failed to load PDF</p><br>" + emailBody + @"
                    <br><p>Please contact any persons in charge of dataset</p>
                    <p>To inform them of the error</p>";
                    }

                    EmailHelper.SendEmailWithPdfAsync(
                     smtpHost: _configuration["Secrets:SmtpHost"],
                     smtpPort: Convert.ToInt16(_configuration["Secrets:SmtpPort"]),
                     smtpUser: _configuration["Secrets:SmtpUser"],
                     smtpPass: _configuration["Secrets:SmtpPass"],
                     fromEmail: _configuration["Secrets:FromEmail"],
                     toEmail: _configuration["Secrets:ToEmail"],
                     subject: $"{dto.Name} Report",
                     body: emailBody,
                     pdfBytes: result.pdfBytes!,
                     pdfFileName: "Report.pdf");

                    doc.PdfDownloaded = true;
                }
                catch (Exception ex)
                {
                    // Log the error for that single document
                    doc.Status = $"Error: {ex.Message}";
                }
            });

            // Run all document tasks in parallel
            await Task.WhenAll(processingTasks);

            // Save database changes after all have completed
            await _context.SaveChangesAsync();
        }
    }
}
