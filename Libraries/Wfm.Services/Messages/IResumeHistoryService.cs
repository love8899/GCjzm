using System.Linq;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial interface IResumeHistoryService
    {
        void InsertResumeHistory(ResumeHistory resumeHistory);

        void UpdateResumeHistory(ResumeHistory resumeHistory);

        ResumeHistory GetResumeHistoryById(int resumeHistoryId);

        IQueryable<ResumeHistory> GetAllResumeHistoriesAsQueryable();

        IQueryable<ResumeHistory> GetAllResumeHistoriesByResume(int resumeId);
    }
}
