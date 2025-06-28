using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Services.TimeSheet
{
    public partial interface IWorkTimeService
    {

        #region CRUD

        void InsertCandidateWorkTime(CandidateWorkTime candidateWorkTime);

        void UpdateCandidateWorkTime(CandidateWorkTime candidateWorkTime);

        void DeleteCandidateWorkTime(CandidateWorkTime candidateWorkTime);

        // Insert or Update, for import and manual, as well as missing hour
        int InsertOrUpdateWorkTime(int jobOrderId, int candidateId, DateTime startDate, decimal hours, string source, string note = null, bool logging = true, bool overwriteOrig = false);

        // caulcaute OT
        void CalculateOTforWorktimeWithinDateRange(DateTime startDate, DateTime endDate);

        #endregion


        #region CandidateWorkTime

        CandidateWorkTime GetWorkTimeById(int id);

        CandidateWorkTime GetWorkTimeByCandidateIdAndDate(int candidateId, DateTime selectedDate);

        CandidateWorkTime GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate, bool includeMatched = false);

        #endregion


        #region LIST

        IList<CandidateWorkTime> GetWorkTimeByIds(int[] ids);

        IQueryable<CandidateWorkTime> GetWorkTimeByCandidateIdAsQueryable(int candidateId = 0);

        IQueryable<CandidateWorkTime> GetWorkTimeByStartEndDateAsQueryable(DateTime fromDate, DateTime toDate, bool includeMatched = false);

        IQueryable<CandidateWorkTime> GetWorkTimeNotFullyInvoicedByStartEndDateAsQueryable(DateTime fromDate, DateTime toDate);

        IQueryable<CandidateWorkTime> GetAllWorkTimeByJobOrderAndDateAsQueryable(int jobOrderId, DateTime selectedDate,bool includeVoided = false);

        IQueryable<CandidateWorkTime> GetAllWorkTimeByJobOrderAndStartEndDateAsQueryable(int jobOrderId, DateTime fromDate, DateTime toDate);

        IQueryable<CandidateWorkTime> GetAllCandidateWorkTimeAsQueryable(bool isAccountBased = true);


        // for client
        IQueryable<CandidateWorkTime> GetAllCandidateWorkTimeByAccountAsQueryable(Account account = null, bool showRejected = false);
        IQueryable<CandidateWorkTime> GetOpenCandidateWorkTimeByAccountForApproval(DateTime startDate,Account account = null, bool submittedOnly = false);
        IQueryable<CandidateWorkTime> GetOpenCandidateWorkTimeByAccountForApprovalByWeekStartDate(DateTime weekStartDate, Account account = null, bool submittedOnly = false); 
        IQueryable<CandidateWorkOverTime> GetCandidateOvertime(int[] candidateIds, int[] jobOrerId, DateTime startDate, DateTime endDate);
        IQueryable<Alerts> GetCandidateUnacknowledgedAlerts(int[] candidateIds);
        bool RejectWorkTimeEntry(int id, string reason, Account currentAccount);
        bool ApproveWorkTimeEntry(int id, Account currentAccount, out string errorMessage);
        bool SignWorkTimeEntry(int id, Account currentAccount, out string errorMessage);
        IQueryable<EmployeeWorkTimeApproval> GetEmployeeWorkTimeApprovalAsQueryable(DateTime startDate, DateTime endDate, Account account = null, bool showRejected = false);
        IQueryable<EmployeeWorkTimeApproval> GetEmployeeWorkTimeApprovalAsQueryable(DateTime weekStartDate, Account account = null, bool showRejected = false,
            int? candidateId = null, bool submittedOnly = false);

        // expected time sheets, by placement
        IQueryable<EmployeeWorkTimeApproval> GetExpectedTimeSheets(Account account, DateTime weekStartDate, bool startedOnly = true);

        IQueryable<EmployeeWorkTimeApproval> GetDailyTimeSheetsByAccountAsQueryable(Account account = null, bool showRejected = false);

        #endregion



        #region Calculation the actual work time

        IList<string> CaptureCandidateWorkTime(DateTime startDate, DateTime endDate, Account account = null, bool includePlaced = false, bool includeNotPlaced = false);

        Task<IList<string>> CaptureCandidateWorkTimeAsync(DateTime startDate, DateTime endDate, Account account = null, bool includePlaced = false, bool includeNotPlaced = false);

        void CalculateJobOrderCandidateWorkTime(JobOrder jobOrder, DateTime startDate, DateTime endDate, Account account = null, bool cleanupWorkTimes = true, bool includePlaced = false, bool includeNotPlaced = false);

        decimal GetMinWorkHoursForMealBreak(int jobOrderId);
        CandidateWorkTime PrepareCandidateWorkTimeByJobOrderAndDate(Account account, JobOrder jobOrder, DateTime nDate);
        void CalculateWorkTimeForCandidates(IList<int> candidateIdList, CandidateWorkTime cwt);

        void CalculateWorkTimeForOneCandidate(CandidateWorkTime cwt, InOutTimes inOutTimes, DateTime scanStartDateTime, DateTime scanEndDateTime);

        void CalculateAndSaveWorkTime(CandidateWorkTime candidateWorkTime,bool setLocked=false);

        void RemoveOtherMatchedWorkTimes(CandidateWorkTime cwt, bool includeSubmitted = true);
        void RemoveMatchedWorktimeForAfterFactTimeoffBooking(int candidateId, DateTime beginDate, DateTime endDate);

        bool IsValidWorkTime(CandidateWorkTime cwt);

        decimal CalculateNetWorkTime(decimal grossWorkTimeInMinutes, decimal minWorkHoursForMealBreak, decimal mealTimeInMinutes, decimal breakTimeInMinutes, decimal adjustmentInMinutes);

        void SetClockTimeStatusByWorkTime(CandidateWorkTime cwt, int statusId = (int)CandidateClockTimeStatus.Processed, bool isForRescheduling = false);

        #endregion


        #region ChangeCandidateWorkTimeStatus

        void ChangeCandidateWorkTimeStatus(CandidateWorkTime canWorkTime, CandidateWorkTimeStatus status, Account currentAccount);

        #endregion

        #region Daily Approval Support
        bool ManualAdjustCandidateWorkTime(int candidateId, int jobOrderId, DateTime startDate, int newJobOrderId, decimal adjustmentMinutes, decimal netHours, string note);
        int SaveManualCandidateWorkTime(CandidateWorkTime model, string source, int statusId = (int)CandidateWorkTimeStatus.PendingApproval);
        #endregion

        #region Alert missed clocking in/out

        void AlertMissingClockInOut();

        #endregion


        #region Candidate Attendance

        IList<int> GetJobOrderPlacedCandidateIds(int jobOrderId, DateTime startDate);

        IList<int>[] GetAttendanceCountByJobOrderAndSelectedWeek(int jobOrderId, DateTime startDate);

        #endregion


        #region time sheet approval

        IQueryable<TimeSheetSummary> GetTimeSheetSummaryForApprovalByWeekStartDate(DateTime weekStartDate);

        void ApproveWorkTimeByCompanyIdAndDateRange(int companyId, int? supervisorId, DateTime startDate, DateTime endDate, int vendorId = 0);

        IQueryable<TimeSheetSummary> GetTimeSheetSummaryForApprovalByDateRange(DateTime startDate, DateTime endDate);


        int[] GetSupervisorIdsWithPendingApproval(DateTime startDate, DateTime endDate, int? companyId);

        #endregion


        #region Invoice

        void SetWorkTimeInvoiceDate(int companyId, DateTime startDate, DateTime endDate);

        Task SetWorkTimeInvoiceDate(string selectedIds);
        #endregion


        #region Client Time Sheet Document

        void InsertClientTimeSheetDocument(ClientTimeSheetDocument clientTimeSheetDoc);

        void DeleteClientTimeSheetDocument(ClientTimeSheetDocument clientTimeSheetDoc);

        ClientTimeSheetDocument GetClientTimeSheetDocumentById(int id);

        IList<ClientTimeSheetDocument> GetAllClientTimeSheetDocumentsByJobOrderIdAndDateRange(int jobOrderId, DateTime startDate, DateTime endDate);

        #endregion

        #region Check Approved Work Time

        bool CheckApprovedWorkTime(int candidateId, int jobOrderId, DateTime time);

        bool CheckApprovedWorkTimeWithinDateRange(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate = null);

        bool IfAllJobOrderWorkTimeApproved(IList<int> jobOrderIds, DateTime refDate);
        
        #endregion


        #region Get Total Hours the candidate worked for the company

        decimal GetCandidateTotalHoursForCompany(int candidateId, int companyId);

        IQueryable<EmployeeTotalHours> GetAllEmployeeTotalHours(DateTime startDate, DateTime endDate, int[] status, int companyId = 0, Account account = null);

        #endregion

    }
}
