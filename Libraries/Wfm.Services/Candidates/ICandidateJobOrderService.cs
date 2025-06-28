using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Services.Candidates
{

    /// <summary>
    /// CandidateJobOrder Service
    /// </summary>
    public partial interface ICandidateJobOrderService
    {

        #region CRUD

        /// <summary>
        /// Inserts the candidate JobOrder.
        /// </summary>
        /// <param name="candidtateJobOrder">The candidtate job order.</param>
        void InsertCandidateJobOrder(CandidateJobOrder candidtateJobOrder, bool logging = true);

        /// <summary>
        /// Updates the candidate JobOrder.
        /// </summary>
        /// <param name="candidtateJobOrder">The candidtate job order.</param>
        void UpdateCandidateJobOrder(CandidateJobOrder candidtateJobOrder, bool logging = true);

        /// <summary>
        /// Deletes the candidate job order.
        /// </summary>
        /// <param name="candidateJobOrder">The candidate job order.</param>
        void DeleteCandidateJobOrder(CandidateJobOrder candidateJobOrder, bool logging = true);

        void InsertOrUpdateCandidateJobOrder(int jobOrderId, int candidateId, DateTime startDate, int statusId, DateTime? endDate, int enteredBy, bool logging = true);

        #endregion

        #region CandidateJobOrder

        /// <summary>
        /// Gets the candidate JobOrder by candidateJobOrderId.
        /// </summary>
        /// <param name="id">The candidate job order id.</param>
        /// <returns>CandidateJobOrder</returns>
        CandidateJobOrder GetCandidateJobOrderById(int id);

        IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersAsQueryable(DateTime? refDate = null, int statusId = 0);

        CandidateJobOrder GetCandidateJobOrderByJobOrderIdAndCandidateId(int jobOrderId, int candidateId);

        IList<CandidateJobOrder> GetCandidateJobOrderByJobOrderIdAndDateRange(int jobOrderId, DateTime startDate, DateTime endDate);

        IList<CandidateJobOrder> GetAllCandidateJobOrdersByJobOrderIdAndCandidateId(int jobOrderId, int candidateId);

        IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersByJobOrderIdAndCandidateIdAndDate(int jobOrderId, int candidateId, DateTime earliestDate);

        bool IsCandidateInJobOrderPipeline(int jobOrderId, int candidateId);

        bool IsCandidatePlacedInJobOrderWithinDateRange(int jobOrderId, int candidateId, DateTime startDate, DateTime endDate);

        void UpdateCandidateJobOrderRatingValue(int candidateJobOrderId, decimal rating);

        IEnumerable<Tuple<DateTime, bool>> GetJobOrderDailyFlags(int jobOrderId, DateTime startDate, DateTime endDate);

        IEnumerable<CandidateJobOrder> GetDailyPlacement(IEnumerable<CandidateJobOrder> placement, DateTime startDate, DateTime endDate);

        IEnumerable<CandidateJobOrder> GetActualDailyPlacement(int jobOrderId, DateTime startDate, DateTime endDate);

        #endregion

        #region LIST

        /// <summary>
        /// Gets the candidate Joborder by JobOrderId.
        /// </summary>
        /// <param name="jobOrderId">The jobOrderId.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns>IPagedList CandidateJobOrder</returns>
        IPagedList<CandidateJobOrder> GetCandidateJobOrderByJobOrderId(int jobOrderId, DateTime? refDate = null, int pageIndex = 0, int PageSize = int.MaxValue);



        /// <summary>
        /// Gets the CandidateJobOrder by JobOrderId and StatusId.
        /// </summary>
        /// <param name="jobOrderId">The JobOrderId.</param>
        /// <param name="jobOrderStatusId">The JobOrder Status Id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<CandidateJobOrder> GetCandidateJobOrderByJobOrderIdAndStatusId(int jobOrderId, int jobOrderStatusId, int pageIndex = 0, int PageSize = int.MaxValue);

        /// <summary>
        /// Gets the candidate job order by CandidateId.
        /// </summary>
        /// <param name="candidateId">The candidate id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<CandidateJobOrder> GetCandidateJobOrderByCandidateId(int candidateId = 0, int pageIndex = 0, int PageSize = int.MaxValue);

        /// <summary>
        /// Gets the candidate JobOrder by CandidateId AND StatusId.
        /// </summary>
        /// <param name="candidateId">The candidate id.</param>
        /// <param name="statusId">The status id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<CandidateJobOrder> GetCandidateJobOrderByCandidateIdAndStatusId(int candidateId, int statusId, int pageIndex = 0, int PageSize = int.MaxValue);

        IQueryable<CandidateJobOrder> GetCandidateJobOrderByCandidateIdAsQueryable(int candidateId);

        IQueryable<CandidateJobOrder> GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(int companyId, DateTime refDate);

        IQueryable<CandidateJobOrder> GetLastCandidateJobOrdersBeforeDateAsQueryable(DateTime refDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed);

        IQueryable<CandidateJobOrder> GetFirstCandidateJobOrdersAfterDateAsQueryable(DateTime refDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed);

        #endregion

        #region Dynamic Job Order

        bool IsCandidateAlreadyPlacedInOtherJobOrderWithinDateRange(int candidateId, DateTime startDate, DateTime? ednDate = null, int oldJobOrderId = 0);
        
        IQueryable<CandidateJobHistory> GetCandidateJobOrderHistoryByCandidateIdAsQueryable(int candidateId);
        
        IEnumerable<EmployeeTimeChartHistory> GetCandidateTimeSheetByCandidateIdAndJobOrderId(int candidateId, int jobOrderId);
        
      //  IEnumerable<Candidate_Payment_History> GetCandidate_Payment_HistoryByCandidateId(int candidateId);
        
        //IEnumerable<EmployeeTimeChartHistory> GetTimesheetAndPaymentByCandidateId(int candidateId, out IEnumerable<Candidate_Payment_History> paymentHistory);
        
        void SetEndDateAfterClosingJobOrder(int jobOrderId,DateTime endDate);
        
        #endregion


        #region Opening vs Placed

        int GetTotalCandidateByHired(DateTime datetime);

        int GetNumberOfPlacedCandidatesByJobOrder(int jobOrderId, DateTime refDate);

        int GetNumberOfPlacedCandidatesByJobPosting(int jobPostingId, DateTime refDate);

        bool IsJobAlreadyFilled(int id, DateTime startDate, DateTime? endDate, out DateTime? filledDate, bool byJobOrder, int? refCandidateId =  null);

        string IsJobOrderOrJobPostingAlreadyFilled(int jobOrderId, DateTime startDate, DateTime? endDate, int? refCandidateId = null);

        IList<int> GetOpeningNumbersPerDay(int id, DateTime startDate, DateTime? endDate, out DateTime firstDate, bool byJobOrder = true);

        IList<int> GetPlacedNumbersPerDay(int id, DateTime startDate, DateTime? endDate, out DateTime firstDate, bool byJobOrder = true, int? refCandidateId = null);

        IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersByDateRangeAsQueryable(DateTime startDate, DateTime? endDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed);

        IQueryable<CandidateAvailableDays> GetAvailableDaysOfCandidateWithinDataRange(DateTime startDate, DateTime endDate);

        #endregion


        #region validation for placement

        string IsJobOrderValidForPlacement(int jobOrderId, DateTime startDate, DateTime? endDate);

        string IsCandidateQualifiedForJobOrder(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate, int? statusId = null);

        bool HasCandidatePassedAllTestsRequiredByJobOrder(int candidateId, int jobOrderId);

        string AnyApprovedWorkTimeWithinDateRange(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate);

        string AnyOtherPlacementWithinDateRange(int newJobOrderId, int candidateId, DateTime startDate, DateTime? endDate, int oldJobOrderId = 0);

        #endregion


        #region Placement
        string ApplyPlacementRules(CandidateJobOrder newCjo, int oldJobOrderId = 0);

        string CreateOrSavePlacements(CandidateJobOrder newCjo, bool forecePlacementRulesOnUpdate = false);

        string CreateNewPlacement(CandidateJobOrder newCjo, int oldJobOrderId = 0, bool addToCompanyPool = true);

        string UpdateExistingPlacement(CandidateJobOrder newCjo, bool keepLeftOver = true);

        string RemovePlacement(CandidateJobOrder existingCjo);

        string SetCandidateJobOrderToStandbyStatus(int candidateId, int jobOrderId, int existingStatusId,
            DateTime startDate, DateTime? endDate, int newStatusId = (int)CandidateJobOrderStatusEnum.NoStatus);

        string SetCandidateJobOrderToNoStatus(int candidateId, int jobOrderId, int existingStatusId, DateTime startDate, DateTime? endDate);

        string SetCandidateJobOrderToPlaced(int candidateId, int jobOrderId, DateTime existingStartDate, DateTime? existingEndDate, int existingStatusId, DateTime startDate, DateTime? endDate);

        bool CheckPlacementForOverlaps(int candidateId, int newJobOrderId, int oldJobOrderId, DateTime fromDate, DateTime? toDate, DateTime shiftStartTime, DateTime shiftEndTime,
                                       out string Message, out bool HasWarning, out DateTime? EndDate );

        string RemoveCandidateFromAllPipelines(int candidateId, DateTime startDate, int? companyId = null);

        #endregion


        #region DailyAttendance

        IList<DailyAttendanceList> GetDailyAttendanceList(DateTime? refDate,DateTime? clientDateTime, Account account = null);

        IQueryable<DailyPlacement> GetDailyPlacments(IQueryable<CandidateJobOrder> source, DateTime startDate, DateTime endDate, bool workDayOnly = true);

        #endregion
    }
}
