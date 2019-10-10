using System;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;

namespace BusinessLibrary.Repositories
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it
                client.Send(mail);

                client.Dispose();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}