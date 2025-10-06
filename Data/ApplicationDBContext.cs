using Microsoft.EntityFrameworkCore;
using PDF_Report_Downloader.Models;
using PDF_Report_Downloader.Models.DTO;
using System.Collections.Generic;

namespace PDF_Report_Downloader.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReportDTO> Reports { get; set; }
    }
}