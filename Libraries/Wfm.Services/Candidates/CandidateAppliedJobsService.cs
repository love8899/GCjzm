using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public class CandidateAppliedJobsService:ICandidateAppliedJobsService
    {
        #region Fields
        private readonly IRepository<CandidateAppliedJobs> _candidateAppliedJobsRepository;
        #endregion

        #region Ctor
        public CandidateAppliedJobsService(IRepository<CandidateAppliedJobs> candidateAppliedJobsRepository)
        {
            _candidateAppliedJobsRepository = candidateAppliedJobsRepository;
        }
        #endregion
        public void Create(Core.Domain.Candidates.CandidateAppliedJobs entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CandidateAppliedJobs");
            entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateAppliedJobsRepository.Insert(entity);
        }

        public Core.Domain.Candidates.CandidateAppliedJobs Retrieve(int id)
        {
            if (id == 0)
                return null;
            var entity = _candidateAppliedJobsRepository.GetById(id);
            return entity;
        }

        public void Update(Core.Domain.Candidates.CandidateAppliedJobs entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CandidateAppliedJobs");
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateAppliedJobsRepository.Update(entity);
        }

        public IQueryable<Core.Domain.Candidates.CandidateAppliedJobs> GetAllCandidateAppliedJobs(int candidateId)
        {
            var result = _candidateAppliedJobsRepository.TableNoTracking.Where(x => x.CandidateId == candidateId);
            return result;
        }

        public void CandidateAppliedJob(int candidateId, int jobOrderId)
        {
            CandidateAppliedJobs entity = new CandidateAppliedJobs();
            entity.CandidateId = candidateId;
            entity.JobOrderId = jobOrderId;
            Create(entity);
        }
        public bool HasCandidateAppliedTheJob(int candidateId, int jobOrderId)
        {
            int num = _candidateAppliedJobsRepository.TableNoTracking.Where(x => x.CandidateId == candidateId && x.JobOrderId == jobOrderId).Count();
            return num > 0;
        }
    }
}
