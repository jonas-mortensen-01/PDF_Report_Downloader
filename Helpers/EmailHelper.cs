using System.Net;
using System.Net.Mail;


namespace PDF_Report_Downloader.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmailWithPdfAsync(
            string smtpHost,
            int smtpPort,
            string smtpUser,
            string smtpPass,
            string fromEmail,
            string toEmail,
            string subject,
            string body,
            byte[] pdfBytes,
            string pdfFileName)
        {
            Console.WriteLine("[Email] Starting email send process...");

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;

                // Write PDF bytes to a temporary file
                string tempPath = Path.Combine(Path.GetTempPath(), pdfFileName);
                File.WriteAllBytesAsync(tempPath, pdfBytes);

                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    fs.Write(pdfBytes, 0, pdfBytes.Length);
                    fs.Position = 0;

                    var attachment = new Attachment(fs, pdfFileName, "application/pdf");
                    message.Attachments.Add(attachment);

                    using var client = new SmtpClient(smtpHost, smtpPort)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(smtpUser, smtpPass)
                    };

                    client.Send(message);
                }

                // Optional: cleanup temporary file
                File.Delete(tempPath);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[Email][SMTP Error] {ex.StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email][Unexpected Error] {ex.Message}");
                throw;
            }
        }
    }


}