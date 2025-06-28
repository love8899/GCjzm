using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public interface ICandidateAppliedJobsService
    {
        #region CRUD
        void Create(CandidateAppliedJobs entity);
        CandidateAppliedJobs Retrieve(int id);
        void Update(CandidateAppliedJobs entity);
        #endregion

        #region Methods
        IQueryable<CandidateAppliedJobs> GetAllCandidateAppliedJobs(int candidateId);
        void CandidateAppliedJob(int candidateId, int jobOrderId);
        bool HasCandidateAppliedTheJob(int candidateId, int jobOrderId);
        #endregion
    }
}
