using System;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace BusinessLibrary.Repositories
{
    public class EmailSender : IEmailSenderUpdated
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task<string> EmailVerificationMessageAsync(string callbackUrl, HttpRequest Request, IUrlHelper Url)
        {
            try
            {
                var privacyUrl = Url.Action(new UrlActionContext
                {
                    Action = "Privacy",
                    Controller = "Home",
                    Values = null,
                    Protocol = Request.Scheme = "https",
                    Host = "localhost:44351"
                });

                return Task.FromResult("<h2>Email confirmation</h2>" +
                    "<h4>Please confirm your email</h4>" +
                    "<hr />" +
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>." +
                    "<br /><br /><br /><br />" +
                    $"<small>We respect your privacy and will never use any of your information for financial nor beneficial purposes. " +
                    $"For more information click <a href='{HtmlEncoder.Default.Encode(privacyUrl)}'>here</a>.</small>");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sends a mail to the given email to verify.
        /// </summary>
        /// <param name="email">The email to send the mail to.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="htmlMessage">The message of the mail.</param>
        /// <returns></returns>
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