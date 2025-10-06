using Microsoft.EntityFrameworkCore;
using PDF_Report_Downloader.Data;
using PDF_Report_Downloader.Services;

namespace PDF_Report_Downloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=SPAC-PF40DSBX\\SQLEXPRESS;Database=PDF_Report_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true")
                .Options;

            using var context = new ApplicationDbContext(options);
            var service = new DocumentProcessingService(context);

            _ = service.ProcessPendingDocuments();
        }
    }
}
