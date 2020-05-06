using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Fasetto.Word.Core;
using System;
using System.Diagnostics;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Web.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details)
        {
            // Get the SendGrid key
            var apiKey = IoCContainer.Configuration["SendGridKey"];

            // Create a new SendGrid client
            var client = new SendGridClient(apiKey);

            // From
            var from = new EmailAddress(details.FromEmail, details.FromName);

            // To
            var to = new EmailAddress(details.ToEmail, details.ToName);

            // Subject
            var subject = details.Subject;
            
            // Content
            var content = details.Content;

            // Create email class ready to send
            var msg = MailHelper.CreateSingleEmail(
                from, 
                to, 
                subject, 
                // Plain content
                details.IsHTML ? null : details.Content, 
                // HTML content
                details.IsHTML ? details.Content : null);

            // Finally, send the email...
            var response = await client.SendEmailAsync(msg);

            //If we succeded...
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                // Return successfull response
                return new SendEmailResponse();

            // Otherwise, it failed...
            try
            {
                // Get the result in the body
                var bodyResult = await response.Body.ReadAsStringAsync();

                // Deserialize the response
                var sendGridResponse = JsonConvert.DeserializeObject<SendGridResponse>(bodyResult);

                // Add any errors to the response
                var errorResponse = new SendEmailResponse
                {
                    Errors = sendGridResponse?.Errors.Select(f => f.Message).ToList()
                };

                // Make sure we have at least one error
                if (errorResponse.Errors == null || errorResponse.Errors.Count == 0)
                    // Add an unknown error
                    // TODO: Localization
                    errorResponse.Errors = new List<string>(new[] { "Unknown error from email service. Please contact Fasetto support" });

                // Return errors
                return errorResponse;

            }
            catch(Exception ex)
            {
                // TODO: Localization

                if (Debugger.IsAttached)
                {
                    var error = ex;
                    Debugger.Break();
                }

                // If something unexpecting happened, return message
                return new SendEmailResponse
                {
                    Errors = new List<string>(new[] { "Unknown error occurred" })
                };
            }

        }

        
    }
}
