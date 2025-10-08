using Microsoft.EntityFrameworkCore;
using PDF_Report_Downloader.Data;
using PDF_Report_Downloader.Services;
using Microsoft.Extensions.Configuration;

namespace PDF_Report_Downloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
           // Only JSON support
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)  // optional, base folder for appsettings.json
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=SPAC-PF40DSBX\\SQLEXPRESS;Database=PDF_Report_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true")
                .Options;

            using var context = new ApplicationDbContext(options);
            var service = new DocumentProcessingService(configuration, context);

            _ = service.ProcessPendingDocuments();
        }
    }
}
