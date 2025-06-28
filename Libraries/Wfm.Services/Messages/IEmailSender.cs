using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial interface IEmailSender
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="candidate">The candidate.</param>
        void SendEmail(MessageTemplate message, Candidate candidate);


        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyToAddress">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachmentFilePath">Attachment file content bytes</param>
        void SendEmail(EmailAccount emailAccount, string subject, string body,
                       string fromAddress, string fromName, string toAddress, string toName,
                       string replyToAddress = null, string replyToName = null,
                       IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
                       string attachmentFilePath = null, string attachmentFileName = null, byte[] attachmentFile = null,
                       string attachmentFileName2=null,byte[] attachmentFile2=null,
                       string attachmentFileName3 = null,byte[] attachmentFile3=null,
                       bool includeLogo = false, string logoFilePath = null
            );
    }
}
