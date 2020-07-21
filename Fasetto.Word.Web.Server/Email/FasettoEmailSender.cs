using Fasetto.Word.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fasetto.Word.Web.Server
{
    /// <summary>
    /// Handles sending emails specific to Fasetto Word server
    /// </summary>
    public static class FasettoEmailSender
    {
        /// <summary>
        /// Sends a verification email to the specified user
        /// </summary>
        /// <param name="displayName"> The user display name (typically first name) </param>
        /// <param name="email"> The users email to be verified </param>
        /// <param name="verificationUrl"> The URL the user needs to click to verify the email </param>
        /// <returns></returns>
        public static async Task<SendEmailResponse> SendUserVerificationEmailAsync(string displayName, string email, string verificationUrl)
        {
           
            return await IoC.EmailTemplateSender.SendGeneralEmailAsync(new SendEmailDetails
            {
                IsHTML = true,
                FromEmail = IoCContainer.Configuration["FasettoSettings:SendEmailFromEmail"],
                FromName = IoCContainer.Configuration["FasettoSettings:SendEmailFromName"],
                ToEmail = email,
                ToName = displayName,
                Subject = "Verify Your Email - Fasetto Word"
            },
            "Verify email",
            $"Hi {displayName ?? "User"},",
            "Thanks for creating an account with us.<br/>To continue please verify your email.",
            "Verify Email",
            verificationUrl
            );
        }
    }
}
