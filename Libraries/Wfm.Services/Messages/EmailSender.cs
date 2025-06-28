using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Messages;
using System.Reflection;


namespace Wfm.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class EmailSender : IEmailSender
    {
        #region Fields

        private readonly IEmailAccountService _emailAcccountService;
        private EmailAccount emailAccount;

        #endregion

        #region Cotr

        public EmailSender(IEmailAccountService emailAccountService)
        {
            _emailAcccountService = emailAccountService;
            emailAccount = _emailAcccountService.GetEmailAccountById(1);
        }

        #endregion
        
        public void SendEmail(MessageTemplate message, Candidate candidate)
        {
            SendEmail(emailAccount, message.Subject, message.Body, message.BccEmailAddresses,
                      emailAccount.DisplayName,
                      candidate.Email,
                      String.Concat(candidate.FirstName , " " , candidate.LastName) );
        }


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
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachmentFilePath">Attachment file content bytes</param>
        public void SendEmail(EmailAccount emailAccount, string subject, string body,
                              string fromAddress, string fromName, string toAddress, string toName,
                              string replyTo = null, string replyToName = null,
                              IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
                              string attachmentFilePath = null, string attachmentFileName = null, byte[] attachmentFile = null,
                              string attachmentFileName2 = null, byte[] attachmentFile2 = null, 
                              string attachmentFileName3 = null, byte[] attachmentFile3 = null,
                              bool includeLogo = false, string logoFilePath = null)
        {
            using (var message = new MailMessage())
            {

#if DEBUG
                toAddress = ConfigurationManager.AppSettings["EmailTo"];
                cc = null;
                bcc = null;
#endif

                //from, to, reply to
                message.From = new MailAddress(fromAddress, fromName);
                var toList = toAddress.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                toName = toList.Length == 1 ? toName : String.Empty;
                foreach (var address in toList)
                    message.To.Add(new MailAddress(address.Trim(), toName));

                if (!String.IsNullOrEmpty(replyTo))
                    message.ReplyToList.Add(new MailAddress(replyTo, replyToName));

                //BCC
                if (bcc != null)
                    foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                        message.Bcc.Add(address.Trim());

                //CC
                if (cc != null)
                    foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                        message.CC.Add(address.Trim());

                //content
                message.Subject = subject;
                message.IsBodyHtml = true;
                // replace new line flag ???
                body = body.Replace("\r\n", "<br/>");

                if (includeLogo)
                {
                    string attachmentPath = logoFilePath;
                    Attachment inline = new Attachment(attachmentPath);
                    inline.ContentDisposition.Inline = true;
                    inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    inline.ContentId = Guid.NewGuid().ToString();
                    inline.ContentType.MediaType = "image/png";
                    inline.ContentType.Name = Path.GetFileName(attachmentPath);

                    message.Attachments.Add(inline);
                    string htmlTag = String.Concat(@"cid:", inline.ContentId);
                    message.Body = HttpUtility.HtmlDecode(body).Replace("%LogoPath%", htmlTag);
                }
                else
                    message.Body = HttpUtility.HtmlDecode(body);

                //create  the file attachment for this e-mail message
                Attachment attachment = null;
                Stream stream = null;
                try
                {
                    if (!String.IsNullOrEmpty(attachmentFileName))
                    {
                        if (!String.IsNullOrEmpty(attachmentFilePath) && Directory.Exists(attachmentFilePath))
                            stream = new FileStream(Path.Combine(attachmentFilePath, attachmentFileName), FileMode.Open, FileAccess.Read);
                        else if (attachmentFile != null && attachmentFile.Length > 0)
                            stream = new MemoryStream(attachmentFile);

                        attachment = new Attachment(stream, attachmentFileName);
                        message.Attachments.Add(attachment);
                    }

                    if (!String.IsNullOrEmpty(attachmentFileName2))
                    {
                        if (attachmentFile2 != null && attachmentFile2.Length > 0)
                            stream = new MemoryStream(attachmentFile2);

                        attachment = new Attachment(stream, attachmentFileName2);
                        message.Attachments.Add(attachment);
                    }

                    if (!String.IsNullOrEmpty(attachmentFileName3))
                    {
                        if (attachmentFile3 != null && attachmentFile3.Length > 0)
                            stream = new MemoryStream(attachmentFile3);

                        attachment = new Attachment(stream, attachmentFileName3);
                        message.Attachments.Add(attachment);
                    }

                    //send email
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                        smtpClient.Host = emailAccount.Host;
                        smtpClient.Port = emailAccount.Port;
                        smtpClient.EnableSsl = emailAccount.EnableSsl;
                        if (emailAccount.UseDefaultCredentials)
                            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                        else
                            smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
                        
//#if !DEBUG
                        smtpClient.Send(message);
//#endif
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }

                }

            }
        }

    }
}
