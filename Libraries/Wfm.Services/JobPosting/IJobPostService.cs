using System.Linq;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.JobOrders;
using System;


namespace Wfm.Services.JobPosting
{
    public partial interface IJobPostService
    {
        #region CRUD

        void InsertJobPost(JP_Core.JobPosting jobPost);
        void UpdateJobPost(JP_Core.JobPosting jobPost);
        void DeleteJobPost(JP_Core.JobPosting jobPost);
        JP_Core.JobPosting RetrieveJobPost(int id);

        JP_Core.JobPosting RetrieveJobPostingByGuid(Guid? guid);
        
        #endregion


        #region List

        IQueryable<JP_Core.JobPosting> GetAllJobPosts(int? companyId = null, bool submittedOnly = false, DateTime? from = null, DateTime? to = null);

        IQueryable<JP_Core.JobPosting> GetAllUnsubmittedJobPosts();

        IQueryable<JobOrder> GetAllJobOrdersByJobPostingId(int jobPostingId);

        IQueryable<JP_Core.JobPosting> GetAllJobPostingByPositionId(int positionId);

        #endregion


        #region Cancel Job Posting

        void CancelJobPosting(JP_Core.JobPosting jobPost,bool changeStatus);
        
        #endregion
    }
}
