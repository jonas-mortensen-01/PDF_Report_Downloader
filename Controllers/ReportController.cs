using System.Net;
using PDF_Report_Downloader.Models;

namespace PDF_Report_Downloader.Controllers
{
    public static class ReportController
    {
        public static async Task<PDFValidation> ValidatePDFAsync(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? pdfUri))
                return new PDFValidation(false, null, "Invalid URL");

            try
            {
                var client = HttpClientProvider._sharedClient;

                // Send HEAD request first to check if the file exists and is a PDF
                using var headRequest = new HttpRequestMessage(HttpMethod.Head, pdfUri);

                var headResponse = new HttpResponseMessage();
                try
                {
                    headResponse = client.Send(headRequest);
                }
                catch (Exception ex) {
                }

                if (!headResponse.IsSuccessStatusCode)
                    return new PDFValidation(false, null, $"HTTP Error {(int)headResponse.StatusCode} - {headResponse.ReasonPhrase}");

                // Check content type if provided
                var contentType = headResponse.Content.Headers.ContentType?.MediaType;
                if (contentType != "application/pdf")
                    return new PDFValidation(false, null, $"URL does not point to a PDF");

                // If HEAD is OK, download the content
                var response = client.GetAsync(pdfUri).Result;
                if (!response.IsSuccessStatusCode)
                    return new PDFValidation(false, null, $"HTTP Error {(int)response.StatusCode} - {response.ReasonPhrase}");

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                // Validate PDF header
                if (pdfBytes.Length < 4 || System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4) != "%PDF")
                    return new PDFValidation(false, null, "File does not have a valid PDF header.");

                return new PDFValidation(true, pdfBytes, "PDF found file attached");
            }
            catch (HttpRequestException ex)
            {
                return new PDFValidation(false, null, $"Request failed: {ex.Message}");
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