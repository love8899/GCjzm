using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial interface IClockTimeService
    {
        #region CRUD

        void Insert(CandidateClockTime candidateClockTime);

        void Update(CandidateClockTime candidateClockTime);

        void Delete(CandidateClockTime candidateClockTime);

        void SoftDelete(CandidateClockTime candidateClockTime);

        string AdvancedInsert(CandidateClockTime candidateClockTime);

        string AdvancedUpdate(CandidateClockTime candidateClockTime);

        void UpdateClockTimeStatus(IList<CandidateClockTime> candidateClockTimes, int statusId = (int)CandidateClockTimeStatus.Processed, int updatedBy = 0, bool isForRescheduling = false);

        #endregion


        #region ClockTime

        CandidateClockTime GetClockTimeById(int id);

        CandidateClockTime GetClockTimeByDeviceUidAndSmartCardUidAndClockTime(string clockDeviceUid, string smartCardUid, DateTime clockInOut);

        #endregion


        #region LIST

        IQueryable<CandidateClockTime> GetAllCandidateClockTimesAsQueryable(bool includeDeleted = false);

        // This is for calculate candidate work time
        List<CandidateClockTime> GetAllClockTimesBySmartCardUidAndClockDeviceUidAndDateTimeRange(string smartCardUid, string deviceUid, DateTime startDateTime, DateTime endDateTime);

        List<CandidateClockTime> GetAllClockTimesByCandidateIdAndLocationIdAndDateTimeRange(int candidateId, int locationId, DateTime? startDateTime, DateTime? endDateTime);

        List<CandidateClockTime> GetAllClockTimesByCandidateIdAndCompanyIdAndDateTimeRange(int candidateId, int companyId, DateTime? startDateTime, DateTime? endDateTime);

        #endregion


        #region Load Punch Clock Time

        IList<string> LoadPunchClockTime(Account account = null);

        string AddClockTime(string clockDeviceUId, int recordNumber, string smartCardUid, DateTime clockInOut,
            string backupFileName, int? candidateId, Account account = null);

        #endregion

    }
}
