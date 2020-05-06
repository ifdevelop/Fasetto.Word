using System.Threading.Tasks;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Sends email using <see cref="IEmailSender"/> and creating the html
    /// email from specific templates
    /// </summary>
    public interface IEmailTemplateSender
    {
        /// <summary>
        /// Sends an email with the given details using the general template
        /// </summary>
        /// <param name="details"> The email message details. Note the Content property is ignored and replaced with the template </param>
        /// <param name="title"> The title of the email</param>
        /// <param name="contennt1"> The first line contents </param>
        /// <param name="content2"> The  second line contents </param>
        /// <param name="buttonText"> The button text </param>
        /// <param name="buttonUrl"> The button URL </param>
        /// <returns></returns>
        Task<SendEmailResponse> SendGeneralEmailAsync(SendEmailDetails details, string title, string contennt1, string content2, string buttonText, string buttonUrl);
    }
}
