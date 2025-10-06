using System.Net;
using System.Net.Mail;

namespace PDF_Report_Downloader.Helpers
{
    public static class EmailHelper
    {
        public static async Task SendEmailWithPdfAsync(string smtpHost, string smtpUser, string smtpPass,
                                               string fromEmail, string toEmail,
                                               string subject, string body, byte[] pdfBytes, string pdfFileName)
        {
            Console.WriteLine("[Email] Starting email send process...");

            try
            {
                using (var message = new MailMessage())
                {
                    Console.WriteLine("[Email] Creating MailMessage...");
                    message.From = new MailAddress(fromEmail);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;

                    Console.WriteLine($"[Email] Preparing PDF attachment: {pdfFileName}, size: {pdfBytes.Length} bytes");
                    using (var stream = new MemoryStream(pdfBytes))
                    {
                        var attachment = new Attachment(stream, pdfFileName, "application/pdf");
                        message.Attachments.Add(attachment);

                        Console.WriteLine($"[Email] Configuring SMTP client: {smtpHost}");
                        using (var client = new SmtpClient(smtpHost))
                        {
                            client.EnableSsl = true; // Usually required
                            client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                            Console.WriteLine("[Email] Sending email...");
                            await client.SendMailAsync(message);
                        }
                    }
                }

                Console.WriteLine("[Email] Email sent successfully!");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[Email][Error] SMTP error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email][Error] Unexpected error: {ex.Message}");
                throw;
            }
        }
    }
}