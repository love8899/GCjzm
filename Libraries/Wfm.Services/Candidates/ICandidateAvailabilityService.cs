using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Services.Candidates
{
    public partial interface ICandidateAvailabilityService
    {

        #region CRUD

        void Insert(CandidateAvailability candidateAvailability);

        void Update(CandidateAvailability candidateAvailability);

        void Delete(CandidateAvailability candidateAvailability);

        #endregion

        CandidateAvailability GetCandidateAvailabilityById(int id);

        IQueryable<CandidateAvailability> GetAllCandidateAvailability(DateTime? startDate = null, DateTime? endDate = null);

        IQueryable<CandidateAvailability> GetCandidateAvailabilityByCandidate(int candidateId, DateTime? startDate = null, DateTime? endDate = null);

        IQueryable<CandidateAvailability> GetCandidateAvailabilityByShift(int shiftId, DateTime? startDate = null, DateTime? endDate = null);

        IQueryable<CandidateAvailability> GetExpectedCandidateAvailabilityByCandidate(int candidateId, DateTime startDate, DateTime endDate, bool byShift = true, bool excludePast = true);

        IEnumerable<CandidateAvailability> GetAllCandidateAvailabilityByCandidate(int candidateId, DateTime startDate, DateTime endDate, bool byShift = true, bool excludePast = true);
    }
}
