using System.Linq;
using System.Collections.Generic;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial interface IClockCandidateService
    {

        #region CRUD

        void Insert(ClockCandidate clockCandidate);

        void Update(ClockCandidate clockCandidate);

        void Delete(ClockCandidate clockCandidate);

        void Delete(int clockDeviceId, int candidateId);

        void InsertOrUpdate(int clockDeviceId, int candidateId);

        void RemoveClockCandidates(int clockDeviceId, IEnumerable<int> candidateIds = null);

        #endregion


        #region ClockCandidate

        ClockCandidate GetClockCandidateById(int id);

        ClockCandidate GetClockCandidate(int clockDeviceId, int candidateId);

        #endregion


        #region List

        IQueryable<ClockCandidate> GetAllClockCandidatesAsQueryable();

        IQueryable<ClockCandidate> GetAllClockCandidatesByClock(int clockDeviceId);

        #endregion


        #region Add / Remove

        bool AddOrRemoveCandidate(CompanyClockDevice clockDevice, HandReader hr, string action, int candidateId);

        #endregion
    }
}
