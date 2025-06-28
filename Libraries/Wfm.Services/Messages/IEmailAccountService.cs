using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        void InsertEmailAccount(EmailAccount emailAccount);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        void UpdateEmailAccount(EmailAccount emailAccount);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        void DeleteEmailAccount(EmailAccount emailAccount);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="id">The email account identifier</param>
        /// <returns>Email account</returns>
        EmailAccount GetEmailAccountById(int id);

        Task<EmailAccount> GetEmailAccountByIdAsync(int id);

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>Email accounts list</returns>
        IList<EmailAccount> GetAllEmailAccounts(bool showInactive = false, bool showHidden = false);

        EmailAccount GetEmailAccountByFranchiseIdAndCode(int franchiseId, string code);

        #region Hourly Limit and Counter

        int? GetHourlySpace(DateTime hourNow);
        
        int? GetHourlyLimit();

        int? GetHourlySentOut(DateTime hourNow);

        int? GetHourlySpaceByAccount(EmailAccount emailAccount, DateTime hourNow);

        int? GetHourlySentOutByAccount(EmailAccount emailAccount, DateTime hourNow);

        int? GetPageSize();

        #endregion
    }
}
