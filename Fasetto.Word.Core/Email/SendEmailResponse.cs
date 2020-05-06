using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// A response from SendEmail call for any <see cref="IEmailSender"/> implementation
    /// </summary>
    public class SendEmailResponse
    {
        /// <summary>
        /// True if the emailwas sent successfully
        /// </summary>
        public bool Successfull => !(Errors.Count > 0);

        /// <summary>
        /// The error messageif the sending failed
        /// </summary>
        public List<string> Errors { get; set; }

    }
}
