using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Logging;

namespace Wfm.Services.Logging
{
   public partial interface IActivityLogService
    {
        #region CRUD

        void InsertActivityLog(ActivityLog activityLog);
        ActivityLog InsertActivityLog(string logTypeName, string comment, params object[] commentParams);
        ActivityLog InsertActivityLog(string logTypeName, string comment, Account account, params object[] commentParams);
        CandidateActivityLog InsertActivityLog(string logTypeName, string comment, Candidate candidate, params object[] commentParams);

        #endregion

        #region ActivityLog

        ActivityLog GetActivityLogById(int id);

        CandidateActivityLog GetCandidateActivityLogById(int id);

        #endregion

        #region PagedList

        IList<ActivityLogType> GetAllActivityLogTypes();

        IQueryable<ActivityLog> GetAllActivityLogAsQueryable(Account account, bool showHidden = false);

        IQueryable<CandidateActivityLog> GetAllCandidateActivityLogAsQueryable(Account account, bool showHidden = false);

        #endregion
    }
}
