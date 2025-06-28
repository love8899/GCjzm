using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Caching;
using Wfm.Services.Events;
using System.Threading.Tasks;

namespace Wfm.Services.Messages
{
    public partial class EmailAccountService : IEmailAccountService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : emailaccount ID
        /// </remarks>
        private const string EMAILACCOUNTS_BY_ID_KEY = "Wfm.emailaccount.id-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string EMAILACCOUNTS_ALL_KEY = "Wfm.emailaccount.all-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string EMAILACCOUNTS_PATTERN_KEY = "Wfm.emailaccount.";

        #endregion

        #region Fields

        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="emailAccountRepository">Email account repository</param>
        /// <param name="eventPublisher">Event published</param>
        public EmailAccountService(
            IRepository<EmailAccount> emailAccountRepository,
            EmailAccountSettings emailAccountSettings,
            IMessageHistoryService messageHistoryService,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher
            )
        {
            _emailAccountRepository = emailAccountRepository;
            _emailAccountSettings = emailAccountSettings;
            _messageHistoryService = messageHistoryService;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void InsertEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            _emailAccountRepository.Insert(emailAccount);

            //cache
            _cacheManager.RemoveByPattern(EMAILACCOUNTS_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(emailAccount);
        }

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void UpdateEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            _emailAccountRepository.Update(emailAccount);

            //cache
            _cacheManager.RemoveByPattern(EMAILACCOUNTS_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(emailAccount);
        }

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void DeleteEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            if (GetAllEmailAccounts().Count == 1)
                throw new WfmException("You cannot delete this email account. At least one account is required.");

            _emailAccountRepository.Delete(emailAccount);

            //cache
            _cacheManager.RemoveByPattern(EMAILACCOUNTS_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(emailAccount);
        }

        #endregion

        #region EmailAccount

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="id">The email account identifier</param>
        /// <returns>Email account</returns>
        public virtual EmailAccount GetEmailAccountById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _emailAccountRepository.GetById(id);

            // Using caching
            string key = string.Format(EMAILACCOUNTS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _emailAccountRepository.GetById(id));
        }

        public virtual Task<EmailAccount> GetEmailAccountByIdAsync(int id)
        {
            return Task.Run(() => GetEmailAccountById(id) );
        }

        public EmailAccount GetEmailAccountByFranchiseIdAndCode(int franchiseId, string code)
        {
            var account = GetAllEmailAccounts().Where(x=>x.FranchiseId == franchiseId&&x.Code == code).FirstOrDefault();
            return account;

        }
        #endregion

        #region LIST

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>Email accounts list</returns>
        public virtual IList<EmailAccount> GetAllEmailAccounts(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _emailAccountRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);

            //query = from ea in query
            //        orderby ea.DisplayOrder, ea.DisplayName
            //        select ea;

            //var emailAccounts = query.ToList();

            //return emailAccounts;


            //using cache
            //-----------------------------
            string key = string.Format(EMAILACCOUNTS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _emailAccountRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);

                query = from ea in query
                        orderby ea.DisplayOrder, ea.DisplayName
                        select ea;

                var emailAccounts = query.ToList();

                return emailAccounts;
            });
        }

        #endregion


        #region Hourly Limit and Counter

        public int? GetHourlySpace(DateTime hourNow)
        {
            if (hourNow == null)
                return null;

            var limit = GetHourlyLimit();
            if (!limit.HasValue)
                return null;
            else
            {
                var sentOut = GetHourlySentOut(hourNow);
                return sentOut.HasValue ? limit - sentOut : limit;
            }
        }


        public int? GetHourlyLimit()
        {
            return _emailAccountSettings.TotalHourlyLimit;
        }


        public int? GetHourlySentOut(DateTime hourNow)
        {
            if (hourNow == null)
                return null;

            return _messageHistoryService.GetNumerOfMessagesByTimeRange(hourNow.AddHours(-1), hourNow);
        }


        public int? GetHourlySpaceByAccount(EmailAccount emailAccount, DateTime hourNow)
        {
            if (hourNow == null)
                return null;

            var limit = emailAccount.HourlyLimit;
            if (!limit.HasValue)
                return null;
            else
            {
                var sentOut = GetHourlySentOutByAccount(emailAccount, hourNow);
                return sentOut.HasValue ? limit - sentOut : limit;
            }
        }


        public int? GetHourlySentOutByAccount(EmailAccount emailAccount, DateTime hourNow)
        {
            if (hourNow == null || emailAccount == null)
                return null;

            return _messageHistoryService.GetNumerOfMessagesByTimeRange(hourNow.AddHours(-1), hourNow, emailAccount.Id);
        }


        public int? GetPageSize()
        {
            return _emailAccountSettings.PageSize;
        }

        #endregion

    }
}
