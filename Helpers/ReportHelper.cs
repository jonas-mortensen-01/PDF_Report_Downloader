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

        public static async Task<PDFValidation> ValidatePDFAsync(string url, string? savePath = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                        return new PDFValidation(false, null, $"HTTP Error {(int)response.StatusCode} - {response.ReasonPhrase}");

                    if (response.Content.Headers.ContentType?.MediaType != "application/pdf")
                        return new PDFValidation(false, null, "URL does not point to a PDF file (Content-Type mismatch).");

                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();

                    if (pdfBytes.Length < 4 || System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4) != "%PDF")
                        return new PDFValidation(false, null, "File does not have a valid PDF header.");

                    if (!string.IsNullOrEmpty(savePath))
                        await File.WriteAllBytesAsync(savePath, pdfBytes);

                    return new PDFValidation(true, pdfBytes, null); // PDF is valid
                }
                catch (HttpRequestException ex)
                {
                    return new PDFValidation(false, null, $"Request error: {ex.Message}");
                }
                catch (TaskCanceledException)
                {
                    return new PDFValidation(false, null, "Request timed out.");
                }
                catch (Exception ex)
                {
                    return new PDFValidation(false, null, $"Unexpected error: {ex.Message}");
                }
            }
        }
    }
}