using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateJobOrderStatusHistoryService
    {

        #region CandidateJobOrderStatusHistory

        CandidateJobOrderStatusHistory GetCandidateJobOrderStatusHistoryById(int id);

        CandidateJobOrderStatusHistory GetLastCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderId(int candidateId, int jobOrderId);

        CandidateJobOrderStatusHistory GetLastCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate);

        #endregion

        #region LIST

        IList<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderId(int candidateId, int jobOrderId);

        IList<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByJobOrderId(int jobOrderId);

        IList<CandidateJobOrderStatusHistory> GetLastCandidateJobOrderStatusByJobOrderId(int jobOrderId);

        IQueryable<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByCandidateIdAsQueryable(int candidateId,bool includeDirectHire=false);

        #endregion


    }
}
