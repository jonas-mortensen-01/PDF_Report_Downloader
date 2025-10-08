using System.Net;
using PDF_Report_Downloader.Models;

namespace PDF_Report_Downloader.Controllers
{
    public static class ReportController
    {
        public static async Task<PDFValidation> ValidatePDFAsync(string url, string? savePath = null)
        {
            try
            {
                if (!Uri.TryCreate("https://investor.cict.com.sg/misc/ar2015.pdf", UriKind.Absolute, out Uri? pdfUri))
                    return new PDFValidation(false, null, "Invalid URL");

                // HttpResponseMessage response = await _sharedClient.GetAsync(url);
                HttpResponseMessage response = Task.Run(() => HttpClientProvider._sharedClient.GetAsync(pdfUri)).Result;

                // var downloadTasks = new List<Task<DownloadTask>>();
                // foreach (int pdfUri in pdfUris)
                // {
                //     downloadTasks.Add(HttpClientProvider._sharedClient.GetAsync(pdfUri));
                // }
                // IEnumerable<HttpResponseMessage> responses = await Task.WhenAll(downloadTasks).Result;

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