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
                using (var message = new MailMessage())
                {

                    message.From = new MailAddress(fromEmail);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    // Use MemoryStream for PDF attachment
                    if (pdfBytes != null && pdfBytes.Length > 0)
                    {
                        using (var pdfStream = new MemoryStream(pdfBytes))
                        {
                            var attachment = new Attachment(pdfStream, pdfFileName, "application/pdf");
                            message.Attachments.Add(attachment);

                            using (var client = new SmtpClient(smtpHost, smtpPort))
                            {
                                client.EnableSsl = true;
                                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                                // Send synchronously
                                client.Send(message);
                            }
                        }
                    }
                    else
                    {
                        using (var client = new SmtpClient(smtpHost, smtpPort))
                        {
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                            // Send the email without attachment
                            client.Send(message);
                        }
                    }
                }

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