using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    /// <summary>
    /// Updated version of IEmailSender that contains a new method.
    /// </summary>
    public interface IEmailSenderUpdated : IEmailSender
    {
        /// <summary>
        /// Contains a default message for the email.
        /// </summary>
        /// <param name="callbackUrl">The callbackUrl to use for the emailmessage.</param>
        /// <returns></returns>
        Task<string> EmailVerificationMessageAsync(string callbackUrl, HttpRequest Request, IUrlHelper Url);
    }
}