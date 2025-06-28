using System;
using System.IO;
using System.Linq;
using System.Transactions;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial class MessageHistoryService :IMessageHistoryService
    {
        #region Fields

        private readonly IRepository<MessageHistory> _messageHistoryRepository;
        private readonly IMessageService _messageService;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        #endregion

        #region Ctor

        public MessageHistoryService(
            IRepository<MessageHistory> messageHistory,
            IMessageService messageService,
            IRepository<QueuedEmail> queuedEmailRepository)
        {
            _messageHistoryRepository = messageHistory;
            _messageService = messageService;
            _queuedEmailRepository = queuedEmailRepository;
        }

        #endregion


        public void InsertMessageHistory(MessageHistory messageHistory)
        {
            if (messageHistory == null)
                throw new ArgumentNullException("messageHistory");

            _messageHistoryRepository.Insert(messageHistory);
        }

        public void InsertMessageHistory(QueuedEmail queuedEmail, string note = null)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            var messageHistory = new MessageHistory
            {
                EmailAccountId = queuedEmail.EmailAccountId,
                Priority = queuedEmail.Priority,
                MailFrom = queuedEmail.From,
                FromName = queuedEmail.FromName,
                FromAccountId = queuedEmail.FromAccountId,
                MailTo = queuedEmail.To,
                ToName = queuedEmail.ToName,
                ToAccountId = queuedEmail.ToAccountId,
                ReplyTo = queuedEmail.ReplyTo,
                ReplyToName = queuedEmail.ReplyToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                Note = note,
                AttachmentFilePath = queuedEmail.AttachmentFilePath,
                AttachmentFileName = queuedEmail.AttachmentFileName,
                AttachmentFile = queuedEmail.AttachmentFile,
                AttachmentTypeId = queuedEmail.AttachmentTypeId,
                AttachmentFileName2 = queuedEmail.AttachmentFileName2,
                AttachmentTypeId2 = queuedEmail.AttachmentTypeId2,
                AttachmentFile2 = queuedEmail.AttachmentFile2,
                AttachmentFileName3 = queuedEmail.AttachmentFileName3,
                AttachmentTypeId3 = queuedEmail.AttachmentTypeId3,
                AttachmentFile3 = queuedEmail.AttachmentFile3,
                IncludeLogo = queuedEmail.IncludeLogo,
                LogoFilePath = queuedEmail.LogoFilePath,
                MessageCategoryId = queuedEmail.MessageCategoryId,
                SentTries = queuedEmail.SentTries,
                SentOnUtc = queuedEmail.SentOnUtc,
                //EmailAccountEmail = queuedEmail
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ConfirmationEmailLinkId = queuedEmail.ConfirmationEmailLinkId
            };

            _messageHistoryRepository.Insert(messageHistory);

            // insert message for every recipient
            _messageService.InsertMessagesForAllRecipients(messageHistory);
        }

        public void InsertTextMessageToMessageHistory(string phoneNumber, string textMessage,Account account,int messageCategoryId)
        {
            var messageHistory = new MessageHistory
            {
                EmailAccountId = 0,
                Priority = 0,
                MailFrom = account.Email,
                FromName = account.FullName,
                FromAccountId = account.Id,
                MailTo = phoneNumber,
                ToName = null,
                ToAccountId = 0,
                ReplyTo = null,
                ReplyToName = null,
                CC = null,
                Bcc = null,
                Subject = textMessage,
                Body = textMessage,
                Note = null,
                IncludeLogo = false,
                MessageCategoryId = messageCategoryId,
                SentTries = 1,
                EnteredBy = account.Id,
                SentOnUtc = DateTime.UtcNow,
                //EmailAccountEmail = queuedEmail
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ConfirmationEmailLinkId = null
            };

            _messageHistoryRepository.Insert(messageHistory);

            // insert message for every recipient
            _messageService.InsertMessagesForAllRecipients(messageHistory,true);
        }

        public void MoveQueuedEmailToMessageHistory(QueuedEmail queuedEmail, string note = null)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            bool deleteAttachment = true;

            using (var scope = new TransactionScope())
            {
                this.InsertMessageHistory(queuedEmail, note);

                // Purge queue and keep it clean for super performance, delete attachment too
                _queuedEmailRepository.Delete(queuedEmail);

                if (deleteAttachment && queuedEmail.AttachmentFilePath != null && queuedEmail.AttachmentFileName != null)
                {
                    // if other queued email(s) using the same attachment
                    bool attachmentStillInNeed = _queuedEmailRepository.TableNoTracking
                                                    .Where(x => x.AttachmentFilePath == queuedEmail.AttachmentFilePath && x.AttachmentFileName == queuedEmail.AttachmentFileName)
                                                    .Any();

                    if (!attachmentStillInNeed)
                    {
                        var attachmentFileName = Path.Combine(queuedEmail.AttachmentFilePath, queuedEmail.AttachmentFileName);
                        if (File.Exists(attachmentFileName))
                            File.Delete(attachmentFileName);
                    }
                }
                
                scope.Complete();
            }
        }


        public void UpdateMessageHistory(MessageHistory messageHistory)
        {
            if (messageHistory == null)
                throw new ArgumentNullException("messageHistory");

            _messageHistoryRepository.Update(messageHistory);
        }


        public MessageHistory GetMessageHistoryById(int messageHistoryId)
        {
            if (messageHistoryId == 0)
                return null;

            return _messageHistoryRepository.GetById(messageHistoryId);

        }

        public bool MessageSentOrNot(string subject)
        {
            var result1 = _messageHistoryRepository.TableNoTracking.Where(x => x.Subject == subject);
            var result2 = _queuedEmailRepository.TableNoTracking.Where(x => x.Subject == subject);
            return result1.Count() > 0 || result2.Count() > 0;
        }
        /// <summary>
        /// Gets all message histories asynchronous queryable.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IQueryable<MessageHistory> GetAllMessageHistoriesAsQueryable(Account account)
        {
            var query = _messageHistoryRepository.Table;

            query = from mh in query
                    where (!account.IsLimitedToFranchises || account.FranchiseId == mh.FranchiseId)
                    orderby mh.CreatedOnUtc descending
                    select mh;

            return query.AsQueryable();
        }


        public IQueryable<MessageHistory> GetAllMessageHistoriesByAccountAsQueryable(Account account)
        {
            var query = _messageHistoryRepository.Table;

            query = query.Where(x => x.ToAccountId == account.Id ||
                                     x.MailTo.Contains(account.Email) ||
                                     x.CC.Contains(account.Email) ||
                                     x.Bcc.Contains(account.Email));

            return query.OrderByDescending(x => x.CreatedOnUtc);
        }


        public IQueryable<MessageHistory> GetAllMessageHistoryByTimeRange(DateTime startTime, DateTime endTime, int? emailAccountId = null)
        {
            var result = Enumerable.Empty<MessageHistory>().AsQueryable();
            
            if (startTime == null || endTime == null)
                return result;

            result = _messageHistoryRepository.TableNoTracking
                     .Where(x => x.SentOnUtc > startTime && x.SentOnUtc <= endTime)
                     .Where(x => !emailAccountId.HasValue || x.EmailAccountId == emailAccountId);

            return result;
        }


        public int GetNumerOfMessagesByTimeRange(DateTime startTime, DateTime endTime, int? emailAccountId = null, bool byRecipient = true)
        {
            var messages = GetAllMessageHistoryByTimeRange(startTime, endTime, emailAccountId).AsEnumerable();

            var result = messages.Count();
            if (byRecipient)
            {
                var numbers = messages.Select(x => 1 + (x.CC != null ? x.CC.Split(';').Count() : 0) + (x.Bcc != null ? x.Bcc.Split(';').Count() : 0));
                result = numbers.Sum();
            }

            return result;
        }

    }
}
