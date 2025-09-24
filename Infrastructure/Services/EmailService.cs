using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty.", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
                throw new InvalidOperationException("FromEmail is not configured in SmtpSettings.");

            var message = new MailMessage();
            message.From = new MailAddress(_smtpSettings.FromEmail);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.EnableSsl = _smtpSettings.EnableSsl;

                await client.SendMailAsync(message);
            }
        }

    }
}
