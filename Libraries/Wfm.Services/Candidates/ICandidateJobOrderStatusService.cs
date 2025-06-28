using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
     public partial interface ICandidateJobOrderStatusService
     {
         #region CandidateJobOrderStatus

         void DeleteCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus);

         void InsertCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus);

         void UpdateCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus);

         #endregion

         #region Methods

         CandidateJobOrderStatus GetCandidateJobOrderStatusById(int id);

         IList<CandidateJobOrderStatus> GetAllCandidateJobOrderStatus(bool showInactive = false);

         #endregion
    }
}
