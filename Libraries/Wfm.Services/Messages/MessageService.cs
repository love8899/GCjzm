using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Events;
using Wfm.Services.Localization;


namespace Wfm.Services.Messages
{
    public partial class MessageService: IMessageService
    {
        #region Fields

        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Candidate> _candidateRepository;

        #endregion


        #region Ctor

        public MessageService(IRepository<Message> messageRepository, IRepository<Account> accountRepository, IRepository<Candidate> candidateRepository)
        {
            this._messageRepository = messageRepository;
            this._accountRepository = accountRepository;
            this._candidateRepository = candidateRepository;
        }

        #endregion


        #region Methods

        public void DeleteMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _messageRepository.Delete(message);
        }


        public void InsertMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _messageRepository.Insert(message);
        }


        public void InsertMessagesForAllRecipients(MessageHistory messageHistory,bool byTextMessage=false)
        {
            if (messageHistory == null)
                return;

            var recipients = new List<string>();

            var delimiters = new char[] { ';', ',' };
            if (!String.IsNullOrEmpty(messageHistory.MailTo))
                recipients.AddRange(messageHistory.MailTo.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

            if (!String.IsNullOrEmpty(messageHistory.CC))
                recipients.AddRange(messageHistory.CC.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

            if (!String.IsNullOrEmpty(messageHistory.Bcc))
                recipients.AddRange(messageHistory.Bcc.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

            foreach (var recipient in recipients.Distinct())
            {
                var IsCandidate = false;
                int accountId = 0;
                if (!byTextMessage)
                    accountId = GetAccountIdByEmail(recipient, out IsCandidate);
                else
                    accountId = GetAccountIdByPhonenumber(recipient, out IsCandidate);
                var message = new Message()
                {
                    MessageHistoryId = messageHistory.Id,
                    AccountId = accountId,
                    IsCandidate = IsCandidate,
                    Recipient = recipient,
                    ByEmail = !byTextMessage,
                    ByMessage = byTextMessage
                };

                InsertMessage(message);
            }
        }

        //public void InsertTextMessageForCandidates(string number, string text,int accountId)
        //{
        //    var candidate = _candidateRepository.TableNoTracking.Where(x => x.MobilePhone == number).FirstOrDefault();
        //    bool isCandidate = false;
        //    if (candidate != null)
        //        isCandidate = true;
        //    var message = new Message()
        //    {
        //        MessageHistoryId = 0,
        //        AccountId = accountId,
        //        IsCandidate = isCandidate,
        //        Recipient = number,
        //        ByEmail = false,
        //        ByMessage = true
        //    };

        //    InsertMessage(message);
            
        //}


        public void UpdateMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("messageCategory");

            _messageRepository.Update(message);
        }


        public Message GetMessageById(int messageId)
        {
            if (messageId == 0)
                return null;

            return _messageRepository.GetById(messageId);
        }


        public int GetTheNumberOfUnreadMessages(int accountId, bool IsCandidate = false, bool messageOnly = false)
        {
            var query = GetMessagesByAccount(accountId, IsCandidate, messageOnly).Where(x => !x.IsRead);

            return query.Count();
        }


        public IQueryable<Message> GetMessagesByAccount(int accountId, bool IsCandidate, bool messageOnly = false)
        {
            if (accountId <= 0)
                return Enumerable.Empty<Message>().AsQueryable();

            var query = _messageRepository.TableNoTracking;

            query = query.Where(x => x.AccountId == accountId && x.IsCandidate == IsCandidate);
            if (messageOnly)
                query = query.Where(x => x.ByMessage);

            return query.OrderByDescending(x => x.MessageHistory.CreatedOnUtc);
        }


        private int GetAccountIdByEmail(string email, out bool IsCandidate)
        {
            var accountId = 0;
            IsCandidate = false;
            if (String.IsNullOrWhiteSpace(email))
                return accountId;

            accountId = _accountRepository.TableNoTracking.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();
            if (accountId == 0)
            {
                accountId = _candidateRepository.TableNoTracking.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault();
                if (accountId > 0)
                    IsCandidate = true;
            }

            return accountId;
        }

        private int GetAccountIdByPhonenumber(string phoneNumber, out bool IsCandidate)
        {
            var accountId = 0;
            IsCandidate = false;
            if (String.IsNullOrWhiteSpace(phoneNumber))
                return accountId;

            accountId = _accountRepository.TableNoTracking.Where(x => x.MobilePhone == phoneNumber).Select(x => x.Id).FirstOrDefault();
            if (accountId == 0)
            {
                accountId = _candidateRepository.TableNoTracking.Where(x => x.MobilePhone == phoneNumber).Select(x => x.Id).FirstOrDefault();
                if (accountId > 0)
                    IsCandidate = true;
            }

            return accountId;
        }
        #endregion
    }
}
