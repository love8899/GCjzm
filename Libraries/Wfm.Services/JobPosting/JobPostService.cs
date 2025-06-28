using System;
using System.Linq;
using Wfm.Core.Data;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.JobOrders;



namespace Wfm.Services.JobPosting
{
    public partial class JobPostService : IJobPostService
    {
        #region Field
        private readonly IRepository<JP_Core.JobPosting> _jobPostRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<JobOrderStatus> _jobOrderStatusRepository;
        #endregion

        #region CTR
        public JobPostService(IRepository<JP_Core.JobPosting> jobPostRepository, IRepository<JobOrder> jobOrderRepository, IRepository<JobOrderStatus> jobOrderStatusRepository)
        {
            _jobPostRepository = jobPostRepository;
            _jobOrderRepository = jobOrderRepository;
            _jobOrderStatusRepository = jobOrderStatusRepository;
        }
        #endregion

        #region CRUD
        public void InsertJobPost(JP_Core.JobPosting jobPost)
        {
            if(jobPost==null)
                throw new ArgumentNullException("jobPost");
            _jobPostRepository.Insert(jobPost);
        }
        public void UpdateJobPost(JP_Core.JobPosting jobPost)
        {
            if(jobPost==null)
                throw new ArgumentNullException("jobPost");
            _jobPostRepository.Update(jobPost);
        }
        public void DeleteJobPost(JP_Core.JobPosting jobPost)
        {
            if(jobPost == null)
                throw new ArgumentNullException("jobPost");
            _jobPostRepository.Delete(jobPost);
        }
        public JP_Core.JobPosting RetrieveJobPost(int id)
        {
            JP_Core.JobPosting jobPost = _jobPostRepository.Table.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            return jobPost;
        }

        public JP_Core.JobPosting RetrieveJobPostingByGuid(Guid? guid)
        {
            if(guid==null)
                return null;
            JP_Core.JobPosting jobPost = _jobPostRepository.Table.Where(x => x.JobPostingGuid==guid.Value && x.IsDeleted == false).FirstOrDefault();
            return jobPost;
        }
        #endregion


        #region List

        public IQueryable<JP_Core.JobPosting> GetAllJobPosts(int? companyId = null, bool submittedOnly = false, 
            DateTime? from = null, DateTime? to = null)
        {
            var jobPosts = _jobPostRepository.Table.Where(x => !x.IsDeleted)
                .Where(x => !companyId.HasValue || x.CompanyId == companyId)
                .Where(x => !submittedOnly || x.IsSubmitted)
                .Where(x => !from.HasValue || (!x.EndDate.HasValue || x.EndDate >= from))
                .Where(x => !to.HasValue || x.StartDate <= to)
                .OrderByDescending(x => x.UpdatedOnUtc);

            return jobPosts;
        }


        public IQueryable<JP_Core.JobPosting> GetAllUnsubmittedJobPosts()
        {
            var jobPosts = _jobPostRepository.TableNoTracking.Where(x => !x.IsDeleted && !x.IsSubmitted);
            
            return jobPosts;
        }


        public IQueryable<JobOrder> GetAllJobOrdersByJobPostingId(int jobPostingId)
        {
            var query = _jobOrderRepository.Table;

            query = query.Where(x => x.JobPostingId == jobPostingId && x.JobOrderStatus.JobOrderStatusName=="Active" && !x.IsDeleted);

            return query;
        }

        public IQueryable<JP_Core.JobPosting> GetAllJobPostingByPositionId(int positionId)
        {
            var query = _jobPostRepository.Table;

            query = query.Where(x=>x.JobPostingStatus.JobOrderStatusName == "Active" && !x.IsDeleted&&x.PositionId==positionId);

            return query;
        }


        #endregion

        #region Cancel JobPosting
        public void CancelJobPosting(JP_Core.JobPosting jobPost,bool changeStatus)
        {
            if (changeStatus)
            {
                var cancelStatus = _jobOrderStatusRepository.TableNoTracking.Where(x => x.JobOrderStatusName == "Canceled").FirstOrDefault();
                jobPost.JobPostingStatusId = cancelStatus == null ? 0 : cancelStatus.Id;
            }
            jobPost.CancelRequestSent = true;
            UpdateJobPost(jobPost);
        }
        #endregion
    }
}
