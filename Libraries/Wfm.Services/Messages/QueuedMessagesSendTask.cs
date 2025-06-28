using System;
using System.Threading.Tasks;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;

namespace Wfm.Services.Messages
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class QueuedMessagesSendTask : IScheduledTask
    {
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailSender _emailSender;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly ILogger _logger;

        public QueuedMessagesSendTask(
            IQueuedEmailService queuedEmailService,
            IEmailSender emailSender,
            IEmailAccountService emailAccountService,
            IMessageHistoryService messageHistoryService,
            ILogger logger)
        {
            this._queuedEmailService = queuedEmailService;
            this._emailSender = emailSender;
            this._emailAccountService = emailAccountService;
            this._messageHistoryService = messageHistoryService;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            var currentHour = DateTime.UtcNow;
            var currentHourSpace = _emailAccountService.GetHourlySpace(currentHour);
            if (!currentHourSpace.HasValue || currentHourSpace > 0)
            {
                var maxTries = 3;
                var defaultPageSize = _emailAccountService.GetPageSize() ?? 5;
                var pageSize = currentHourSpace.HasValue && currentHourSpace < defaultPageSize ? currentHourSpace.Value : defaultPageSize;
                var queuedEmails = _queuedEmailService.SearchEmails(null, null, null, null, true, maxTries, false, 0, pageSize);
                var delimiters = new char[] { ';', ',' };

                foreach (var queuedEmail in queuedEmails)
                {
                    var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc)
                                ? null
                                : queuedEmail.Bcc.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    var cc = String.IsNullOrWhiteSpace(queuedEmail.CC)
                                ? null
                                : queuedEmail.CC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    var num2Send = 1 + (cc != null ? cc.Length : 0) + (bcc != null ? bcc.Length : 0);
                    var space = _emailAccountService.GetHourlySpaceByAccount(queuedEmail.EmailAccount, currentHour);
                    if (!space.HasValue || space.Value >= num2Send)
                    {
                        try
                        {
                            queuedEmail.SentTries = queuedEmail.SentTries + 1;
                            _queuedEmailService.UpdateQueuedEmail(queuedEmail);

                            _emailSender.SendEmail(queuedEmail.EmailAccount,
                                queuedEmail.Subject,
                                queuedEmail.Body,
                                queuedEmail.From,
                                queuedEmail.FromName,
                                queuedEmail.To,
                                queuedEmail.ToName,
                                queuedEmail.ReplyTo,
                                queuedEmail.ReplyToName,
                                bcc,
                                cc,
                                queuedEmail.AttachmentFilePath,
                                queuedEmail.AttachmentFileName,
                                queuedEmail.AttachmentFile,
                                queuedEmail.AttachmentFileName2,
                                queuedEmail.AttachmentFile2,
                                queuedEmail.AttachmentFileName3,
                                queuedEmail.AttachmentFile3, 
                                queuedEmail.IncludeLogo, 
                                queuedEmail.LogoFilePath);

                            // Move success message to message history
                            queuedEmail.SentOnUtc = DateTime.UtcNow;
                            _messageHistoryService.MoveQueuedEmailToMessageHistory(queuedEmail);
                        }
                        catch (Exception exc)
                        {
                            _logger.Error(string.Format("Error on sending e-mail. {0}", exc.Message), exc);
                        }
                    }
                }
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}
