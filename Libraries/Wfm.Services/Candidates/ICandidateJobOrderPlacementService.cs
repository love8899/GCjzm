using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateJobOrderPlacementService
    {
        #region Method
        IList<CandidateJobOrderPlacement> GetCandidateJobOrderPlacement(int candidateId, DateTime startDate);
        #endregion
    }
}
