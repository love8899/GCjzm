using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Data;

namespace Wfm.Services.Candidates
{
    public partial class CandidateJobOrderPlacementService:ICandidateJobOrderPlacementService
    {
        #region field
        private readonly IDbContext _dbContext;
        #endregion
        #region Constructor
        public CandidateJobOrderPlacementService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion
        #region Method
        public IList<CandidateJobOrderPlacement> GetCandidateJobOrderPlacement(int candidateId, DateTime startDate)
        {
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("candidateId", candidateId);
            paras[1] = new SqlParameter("startDate", startDate);
            IList<CandidateJobOrderPlacement> result = _dbContext.SqlQuery<CandidateJobOrderPlacement>("Exec [dbo].[CandidateJobOrderPlacement] @candidateId,@startDate", paras).ToList();
            return result;
        }
        #endregion
    }
}
