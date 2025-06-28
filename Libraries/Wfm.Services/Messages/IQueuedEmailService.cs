using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    public partial interface IQueuedEmailService
    {
        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void InsertQueuedEmail(QueuedEmail queuedEmail);

        void InsertQueuedEmail(IList<QueuedEmail> queuedEmail);

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void UpdateQueuedEmail(QueuedEmail queuedEmail);

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void DeleteQueuedEmail(QueuedEmail queuedEmail, bool deleteAttachment = false);

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        QueuedEmail GetQueuedEmailById(int queuedEmailId);

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        IList<QueuedEmail> GetQueuedEmailsByIds(int[] queuedEmailIds);

        /// <summary>
        /// Search queued emails
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
        /// <returns>Email item collection</returns>
        IPagedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, int maxSendTries,
            bool loadNewest, int pageIndex, int pageSize);


        /// <summary>
        /// Gets all queued emails.
        /// </summary>
        /// <returns></returns>
        IList<QueuedEmail> GetAllQueuedEmails();

        IQueryable<QueuedEmail> GetAllQueuedEmailsAsQueryable(Account account);


        /// <summary>
        /// Delete all queued emails
        /// </summary>
        void DeleteAllEmails();

        #region Mass Email

        void AddQueuedEmail(string from, string to, string cc, string bcc, string subject, string message, int emailAccountId, int priorityId = 5);

        #endregion
    }
}
