using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;
using Wfm.Services.Accounts;


namespace Wfm.Services.Logging
{
    public class AccessLogService :IAccessLogService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        IRepository<AccessLog> _accessLogRepository;
        IRepository<Account> _accountRepository;

        #endregion

        #region Ctor

        public AccessLogService(IWorkContext workContext,
            IRepository<AccessLog> accessLogRepository, IRepository<Account> accountRepository)
        {
            _workContext = workContext;
            _accessLogRepository = accessLogRepository;
            _accountRepository = accountRepository;
        }

        #endregion

        #region CRUD
        public void RecordAccessLog(AccessLog accesslog)
        {
            _accessLogRepository.Insert(accesslog);
        }


        public virtual AccessLog InsertAccessLog(string username, bool isSuccessful, string ipAdress, string userAgent, string comment, params object[] commentParams)
        {
            return InsertAccessLog(username, isSuccessful, ipAdress, userAgent, comment, _workContext.CurrentAccount, commentParams);
        }

        public virtual AccessLog InsertAccessLog(string username, bool isSuccessful, string ipAdress, string userAgent, string comment, Account account, params object[] commentParams)
        {
            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var access = new AccessLog();
            access.AccountId = account == null ? 0 : account.Id;
            access.Username = username;
            access.IsSuccessful = isSuccessful;
            access.IpAdress = ipAdress;
            access.UserAgent = userAgent;
            access.AccountName = account == null ? "" : account.FullName; // account.GetFullName();
            access.FranchiseId = account == null ? 0 : account.FranchiseId;
            access.Description = comment;
            access.CreatedOnUtc = DateTime.UtcNow;
            access.UpdatedOnUtc = DateTime.UtcNow;

            _accessLogRepository.Insert(access);

            return access;
        }
        #endregion

        #region AccessLog
        public AccessLog GetAccessLogById(int accessLogId)
        {
            AccessLog accessLog = (from c in _accessLogRepository.Table
                                   where c.Id == accessLogId
                                   select c).FirstOrDefault();

            if (accessLog != null) return accessLog;

            else return null;
        }

        public IList<string> GetAccessIpAddressesWithinDateRange(DateTime start, DateTime? end = null, bool includeClient = false)
        {
            var accountIds = _accountRepository.TableNoTracking.Where(x => includeClient || !x.IsClientAccount).Select(x => x.Id);
            var query = _accessLogRepository.TableNoTracking
                        .Where(x => x.CreatedOnUtc >= start && x.CreatedOnUtc <= (end.HasValue ? end : DateTime.UtcNow))
                        .Where(x => accountIds.Contains(x.AccountId)).Select(x => x.IpAdress);

            return query.Distinct().ToList();
        }

        #endregion

        #region PagedList
        /// <summary>
        /// Gets all access log.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IPagedList<AccessLog> GetAllAccessLog(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from c in _accessLogRepository.Table
                        orderby (c.CreatedOnUtc) descending
                        select c;

            int total = _accessLogRepository.Table.Count();

            PagedList<AccessLog> accesslogs = new PagedList<AccessLog>(query, pageIndex, PageSize, total);

            return accesslogs;
        }

        /// <summary>
        /// Gets all access log.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IPagedList<AccessLog> GetAllAccessLog(Account account, int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from c in _accessLogRepository.Table
                        where (!account.IsLimitedToFranchises || account.FranchiseId == c.FranchiseId)
                        orderby (c.CreatedOnUtc) descending
                        select c;

            int total = _accessLogRepository.Table.Count();

            PagedList<AccessLog> accessLogs = new PagedList<AccessLog>(query, pageIndex, PageSize, total);

            return accessLogs;
        }

        /// <summary>
        /// Gets all access log asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IQueryable<AccessLog> GetAllAccessLogAsQueryable(Account account, bool showHidden = false)
        {
            var query = _accessLogRepository.Table;

            query = from c in query
                    where (!account.IsLimitedToFranchises || account.FranchiseId == c.FranchiseId)
                    orderby (c.CreatedOnUtc) descending
                    select c;

            return query.AsQueryable();
        }

        #endregion
    }
}
