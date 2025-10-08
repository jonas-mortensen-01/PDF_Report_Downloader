using System.Linq.Expressions;
using PDF_Report_Downloader.Models;
using PDF_Report_Downloader.Models.DTO;

namespace PDF_Report_Downloader.Helpers
{
    public static class ReportHelper
    {
        public static Report? MapReportFromDTO(this ReportDTO dto)
        {
            if (dto == null) return null;

            return new Report
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                PdfUrl = dto.PdfUrl ?? string.Empty,
                PdfDownloaded = dto.PdfDownloaded,
                Status = dto.Status ?? "Unknown"
            };
        }

        public static IEnumerable<Report> MapReportsFromDTOList(IEnumerable<ReportDTO> dtoList)
        {
            List<Report> result = new List<Report>();

            foreach (ReportDTO item in dtoList)
            {
                var resultItem = MapReportFromDTO(item);
                if (resultItem != null) result.Add(resultItem);
            }

            return result;
        }

        public static ReportDTO ToDto(ReportDTO dto)
        {
            return new ReportDTO
            {
                Name = dto.Name,
                PdfUrl = dto.PdfUrl,
                Status = dto.Status
            };
        }
    }
}