using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Messages;
using Wfm.Data;


namespace Wfm.Services.Messages
{
    public partial class QueuedEmailService : IQueuedEmailService
    {
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedEmailRepository">Queued email repository</param>
        /// <param name="eventPublisher">Event published</param>
        public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _queuedEmailRepository = queuedEmailRepository;
            _dbContext = dbContext;
            _dataProvider = dataProvider;
            _commonSettings = commonSettings;
            //_eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Insert(queuedEmail);

            //event notification
            //_eventPublisher.EntityInserted(queuedEmail);
        }

        public virtual void InsertQueuedEmail(IList<QueuedEmail> queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Insert(queuedEmail);

            //event notification
            //_eventPublisher.EntityInserted(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Update(queuedEmail);

            //event notification
            //_eventPublisher.EntityUpdated(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void DeleteQueuedEmail(QueuedEmail queuedEmail, bool deleteAttachment = false)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Delete(queuedEmail);

            if (deleteAttachment && queuedEmail.AttachmentFilePath != null && queuedEmail.AttachmentFileName != null)
            {
                // if other queued email(s) using the same attachment
                bool attachmentStillInNeed = _queuedEmailRepository.Table
                                             .Where(x => x.AttachmentFilePath == queuedEmail.AttachmentFilePath && x.AttachmentFileName == queuedEmail.AttachmentFileName)
                                             .Any();

                if (! attachmentStillInNeed)
                {
                    var attachmentFileName = Path.Combine(queuedEmail.AttachmentFilePath, queuedEmail.AttachmentFileName);
                    if (File.Exists(attachmentFileName))
                        File.Delete(attachmentFileName);
                }
            }

            //event notification
            //_eventPublisher.EntityDeleted(queuedEmail);
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        public virtual QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            return _queuedEmailRepository.GetById(queuedEmailId);

        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        public virtual IList<QueuedEmail> GetQueuedEmailsByIds(int[] queuedEmailIds)
        {
            if (queuedEmailIds == null || queuedEmailIds.Length == 0)
                return new List<QueuedEmail>();

            var query = from qe in _queuedEmailRepository.Table
                        where queuedEmailIds.Contains(qe.Id)
                        select qe;
            var queuedEmails = query.ToList();
            //sort by passed identifiers
            var sortedQueuedEmails = new List<QueuedEmail>();
            foreach (int id in queuedEmailIds)
            {
                var queuedEmail = queuedEmails.Find(x => x.Id == id);
                if (queuedEmail != null)
                    sortedQueuedEmails.Add(queuedEmail);
            }
            return sortedQueuedEmails;
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item list</returns>
        public virtual IPagedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, int maxSendTries,
            bool loadNewest, int pageIndex, int pageSize)
        {
            fromEmail = (fromEmail ?? String.Empty).Trim();
            toEmail = (toEmail ?? String.Empty).Trim();
            
            var query = _queuedEmailRepository.Table;
            if (!String.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!String.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = query.OrderByDescending(qe => qe.Priority);
            query = loadNewest ? 
                ((IOrderedQueryable<QueuedEmail>)query).ThenByDescending(qe => qe.CreatedOnUtc) :
                ((IOrderedQueryable<QueuedEmail>)query).ThenBy(qe => qe.CreatedOnUtc);

            var queuedEmails = new PagedList<QueuedEmail>(query, pageIndex, pageSize);
            return queuedEmails;
        }


        /// <summary>
        /// Gets all queued emails.
        /// </summary>
        /// <returns></returns>
        public virtual IList<QueuedEmail> GetAllQueuedEmails()
        {
            var query = _queuedEmailRepository.Table;

            query = from c in query
                    orderby c.CreatedOnUtc descending
                    select c;

            return query.ToList();
        }

        public IQueryable<QueuedEmail> GetAllQueuedEmailsAsQueryable(Account account)
        {
            var query = _queuedEmailRepository.Table;

            query = from qe in query
                    //where (!account.IsLimitedToFranchises || account.FranchiseId == qe.FranchiseId)
                    orderby qe.CreatedOnUtc descending
                    select qe;

            return query.AsQueryable();
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        public virtual void DeleteAllEmails()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                _dbContext.ExecuteSqlCommand("TRUNCATE TABLE [QueuedEmail]");
            }
            else
            {
                var queuedEmails = _queuedEmailRepository.Table.ToList();
                foreach (var qe in queuedEmails)
                    _queuedEmailRepository.Delete(qe);
            }
        }

        #region Mass Email

        public void AddQueuedEmail(string from, string to, string cc, string bcc, string subject, string message, int emailAccountId, int priorityId = 5)
        {
            if (String.IsNullOrEmpty(from))
                throw new ArgumentNullException("from");
            if (String.IsNullOrEmpty(to) && String.IsNullOrEmpty(cc) && String.IsNullOrEmpty(bcc))
                throw new ArgumentNullException("to or cc or bcc");
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            var email = new QueuedEmail()
            {
                Priority = priorityId,
                From = from,
                To = to,
                CC = cc,
                Bcc = bcc,
                Subject = subject,
                Body = message,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccountId
            };

            this.InsertQueuedEmail(email);
        }

        #endregion
    }
}
