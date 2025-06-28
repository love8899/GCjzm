using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Logging;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Accounts;
using Wfm.Core;
using Wfm.Services.Candidates;

namespace Wfm.Services.Logging
{
    public class ActivityLogService :IActivityLogService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IRepository<CandidateActivityLog> _candidateActivityLogRepository;

        #endregion

        #region Ctor

        public ActivityLogService(IWorkContext workContext,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<Candidate> candidateRepository,
            IRepository<CandidateActivityLog> candidateActivityLogRepository
            )
        {
            _workContext = workContext;
            _activityLogTypeRepository = activityLogTypeRepository;
            _activityLogRepository = activityLogRepository;
            _candidateRepository = candidateRepository;
            _candidateActivityLogRepository = candidateActivityLogRepository;
        }

        #endregion

        #region CRUD

        // Account activity log

        public void InsertActivityLog(ActivityLog activityLog)
        {
            if (activityLog == null)
                throw new ArgumentNullException("activityLog");

            _activityLogRepository.Insert(activityLog);
        }

        public virtual ActivityLog InsertActivityLog(string logTypeName, string comment, params object[] commentParams)
        {
            var account = _workContext != null ? _workContext.CurrentAccount : null;
            return InsertActivityLog(logTypeName, comment, account, commentParams);
        }

        public virtual ActivityLog InsertActivityLog(string logTypeName, string comment, Account account, params object[] commentParams)
        {
            int? accountId = null;
            string accountName = null;
            int? franchiseId = null;
            string franchiseName = null;
            if (account != null)
            {
                accountId = account.Id;
                accountName = account.FullName; // account.GetFullName();
                franchiseId = account.FranchiseId;
                //TO DO: franchiseName = 
            }

            var activityTypes = GetAllActivityLogTypes();
            var activityType = activityTypes.ToList().Find(at => at.ActivityLogTypeName == logTypeName);
            if (activityType == null || !activityType.IsActive)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);


            var activity = new ActivityLog();
            activity.ActivityLogType = activityType;
            activity.AccountId = accountId;
            activity.AccountName = accountName;
            activity.FranchiseId = franchiseId;
            activity.FranchiseName = franchiseName;
            activity.ActivityLogDetail = comment;
            activity.CreatedOnUtc = DateTime.UtcNow;
            activity.UpdatedOnUtc = DateTime.UtcNow;

            _activityLogRepository.Insert(activity);

            return activity;
        }




        // Candidate activity log

        public virtual CandidateActivityLog InsertActivityLog(string logTypeName, string comment, Candidate candidate, params object[] commentParams)
        {
            int? candidateId = null;
            string candidateName = null;
            int? franchiseId = null;
            string franchiseName = null;
            if (candidate != null)
            {
                candidateId = candidate.Id;
                candidateName = candidate.GetFullName();
                franchiseId = candidate.FranchiseId;
                //TO DO: franchiseName = 
            }

            var activityTypes = GetAllActivityLogTypes();
            var activityType = activityTypes.ToList().Find(at => at.ActivityLogTypeName == logTypeName);
            if (activityType == null || !activityType.IsActive)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);


            var activity = new CandidateActivityLog();
            activity.ActivityLogType = activityType;
            activity.CandidateId = candidateId;
            activity.CandidateName = candidateName;
            activity.FranchiseId = franchiseId;
            activity.FranchiseName = franchiseName;
            activity.ActivityLogDetail = comment;
            activity.CreatedOnUtc = DateTime.UtcNow;
            activity.UpdatedOnUtc = DateTime.UtcNow;

            _candidateActivityLogRepository.Insert(activity);

            return activity;
        }


        #endregion

        #region ActivityLog

        public ActivityLog GetActivityLogById(int id)
        {
            if (id == 0)
                return null;

            var query = _activityLogRepository.Table;

            query = from al in query
                    where al.Id == id
                    select al;

            return query.FirstOrDefault();
        }

        public CandidateActivityLog GetCandidateActivityLogById(int id)
        {
            if (id == 0)
                return null;

            var query = _candidateActivityLogRepository.Table;

            query = from al in query
                    where al.Id == id
                    select al;

            return query.FirstOrDefault();
        }

        #endregion

        #region List

        public IList<ActivityLogType> GetAllActivityLogTypes()
        {
            var query = _activityLogTypeRepository.Table;

            query = from at in query
                    orderby at.DisplayOrder, at.ActivityLogTypeName
                    select at;

            return query.ToList();
        }


        /// <summary>
        /// Gets all activity log asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IQueryable<ActivityLog> GetAllActivityLogAsQueryable(Account account, bool showHidden = false)
        {
            var query = _activityLogRepository.TableNoTracking;
            
            if (account.IsLimitedToFranchises)
                query = query.Where(x => x.FranchiseId == account.FranchiseId);

            query = query.OrderByDescending(x => x.CreatedOnUtc);
                                              

            return query;
        }


        public IQueryable<CandidateActivityLog> GetAllCandidateActivityLogAsQueryable(Account account, bool showHidden = false)
        {
            var query = _candidateActivityLogRepository.TableNoTracking;

            if (account.IsLimitedToFranchises)
                query = query.Where(x => x.FranchiseId == account.FranchiseId);

            query = query.OrderByDescending(x => x.CreatedOnUtc);


            return query;
        }


        #endregion
    }
}
