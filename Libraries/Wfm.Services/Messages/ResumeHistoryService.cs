using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial class ResumeHistoryService : IResumeHistoryService
    {
        #region Fields

        private readonly IRepository<ResumeHistory> _resumeHistories;

        #endregion


        #region Ctor

        public ResumeHistoryService(IRepository<ResumeHistory> resumeHistories)
        {
            _resumeHistories = resumeHistories;
        }

        #endregion


        public void InsertResumeHistory(ResumeHistory resumeHistory)
        {
            if (resumeHistory == null)
                throw new ArgumentNullException("resumeHistory");

            resumeHistory.CreatedOnUtc = resumeHistory.UpdatedOnUtc = DateTime.UtcNow;

            _resumeHistories.Insert(resumeHistory);
        }


        public void UpdateResumeHistory(ResumeHistory resumeHistory)
        {
            if (resumeHistory == null)
                throw new ArgumentNullException("resumeHistory");

            resumeHistory.UpdatedOnUtc = DateTime.UtcNow;

            _resumeHistories.Update(resumeHistory);
        }


        public ResumeHistory GetResumeHistoryById(int resumeHistoryId)
        {
            if (resumeHistoryId == 0)
                return null;

            return _resumeHistories.GetById(resumeHistoryId);

        }


        public IQueryable<ResumeHistory> GetAllResumeHistoriesAsQueryable()
        {
            return _resumeHistories.Table;
        }


        public IQueryable<ResumeHistory> GetAllResumeHistoriesByResume(int resumeId)
        {
            return GetAllResumeHistoriesAsQueryable().Where(x => x.ResumeId == resumeId);
        }

    }
}
