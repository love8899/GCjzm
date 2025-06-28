using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;


namespace Wfm.Services.Logging
{
    public partial interface IAccessLogService
    {
        #region CRUD
        void RecordAccessLog(AccessLog accesslog);
        AccessLog InsertAccessLog(string username, bool isSuccessful, string ipAdress, string userAgent, string comment, params object[] commentParams);
        AccessLog InsertAccessLog(string username, bool isSuccessful, string ipAdress, string userAgent, string comment, Account account, params object[] commentParams);
        #endregion

        #region AccessLog
        AccessLog GetAccessLogById(int accessLogId);

        IList<string> GetAccessIpAddressesWithinDateRange(DateTime start, DateTime? end = null, bool includeClient = false);

        #endregion

        #region PagedList
        IPagedList<AccessLog> GetAllAccessLog(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);
        IPagedList<AccessLog> GetAllAccessLog(Account account, int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);
        IQueryable<AccessLog> GetAllAccessLogAsQueryable(Account account, bool showHidden = false); 
        #endregion
    }
}
