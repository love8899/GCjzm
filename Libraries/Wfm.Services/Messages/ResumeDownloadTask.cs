using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using Wfm.Core;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Tasks;


namespace Wfm.Services.Messages
{
    public partial class ResumeDownloadTask : IScheduledTask
    {
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IAttachmentService _attachmentService;
        private readonly IResumeService _resumeService;
        private readonly IEmailReceiver _emailReceiver;
        private readonly ILogger _logger;


        public ResumeDownloadTask(
            IWebHelper webHelper,
            MediaSettings mediaSettings,
            IEmailAccountService emailAccountService,
            IAttachmentService attachmentService,
            IResumeService resumeService,
            IEmailReceiver emailReceiver,
            ILogger logger)
        {
            this._webHelper = webHelper;
            this._mediaSettings = mediaSettings;
            this._emailAccountService = emailAccountService;
            this._attachmentService = attachmentService;
            this._resumeService = resumeService;
            this._emailReceiver = emailReceiver;
            this._logger = logger;
        }


        public virtual void Execute()
        {
            var emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault(x => x.Code == "Resume");
            if (emailAccount != null)
            {
                try
                {
                    var uniqueIds = new List<UniqueId>();
                    var lastUniqueId = (uint)_resumeService.GetLastUniqueId(emailAccount.Email);
                    var batch = (uint)_mediaSettings.ResumeDownloadBatch;
                    var range = new UniqueIdRange(new UniqueId(lastUniqueId + 1), new UniqueId(lastUniqueId + batch));
                    var emails = _emailReceiver.GetEmails(emailAccount, range, ref uniqueIds);
                    for (var i = 0; i < emails.Count; i++)
                        _SaveResume(emailAccount.Email, uniqueIds[i], emails[i]);
                }
                catch (Exception exc)
                {
                    _logger.Error(string.Format("Error on downloading resume: {0}", exc.Message), exc);
                }
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private void _SaveResume(string emailAddress, UniqueId id, MimeMessage email)
        {
            var from = email.From.Mailboxes.First();

            var to = email.To.Mailboxes.Any() ? email.To.Mailboxes.First() : (email.Cc.Mailboxes.Any() ? email.Cc.Mailboxes.First() : null);
            if (to == null)
                return;

            byte[] attachmentBinary = null;
            var attachment = email.Attachments.FirstOrDefault() as MimePart;
            if (attachment != null)
            {
                using (var ms = new MemoryStream())
                {
                    attachment.Content.DecodeTo(ms);
                    attachmentBinary = ms.ToArray();
                }
            }

            var resume = new Resume()
            {
                Account = emailAddress,
                UniqueId = (int)id.Id,
                From = from.Address,
                FromName = from.Name,
                To = to.Address,
                ToName = to.Name,
                Subject = email.Subject,
                Date = email.Date.ToUniversalTime().DateTime,
                Body = email.TextBody,
                HtmlBody = email.HtmlBody,
            };

            if (_mediaSettings.SaveResumeAsFile)
            {
                resume.StoredPath = _GetEmailResumePath(resume.Date);
                var physicalPath = Path.Combine(_mediaSettings.WebRoot, resume.StoredPath);
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);

                // email backup
                var prefix = String.Format("{0}_{1}", emailAddress, id);
                resume.EmailFile = String.Format("{0}{1}", prefix, ".eml");
                email.WriteTo(Path.Combine(physicalPath, resume.EmailFile));

                if (attachmentBinary != null)
                {
                    resume.AttachmentFileName = String.Format("{0}_{1}", prefix, 
                        CommonHelper.ReplaceInvalidCharacters(Regex.Replace(attachment.FileName, @"\s+", ""), "_"));
                    File.WriteAllBytes(Path.Combine(physicalPath, resume.AttachmentFileName), attachmentBinary);

                    // extract text, for searching
                    var fileExtension = Path.GetExtension(resume.AttachmentFileName);
                    if (!String.IsNullOrEmpty(fileExtension))
                        fileExtension = fileExtension.ToLowerInvariant();
                    //resume.AttachmentText = _attachmentService.ExtractFileText(attachmentBinary, fileExtension);
                    try
                    {
                        resume.AttachmentText = _attachmentService.ExtractFileText(attachmentBinary, fileExtension);
                    }
                    catch (Exception exc)
                    {
                        ;   // Skip
                    }
                }
            }

            else
            {
                using (var ms = new MemoryStream())
                {
                    email.WriteTo(ms);
                    resume.EmailBinary = ms.ToArray();
                }
                resume.AttachmentBinary = attachmentBinary;
            }

            _resumeService.InsertOrSkip(resume);
        }


        private string _GetEmailResumePath(DateTime date)
        {
            var year = date.ToString("yyyy");
            var month = date.ToString("MM");
            var day = date.ToString("dd");

            return Path.Combine(_mediaSettings.EmailResumeLocation, year, month, day);
        }
    }
}
