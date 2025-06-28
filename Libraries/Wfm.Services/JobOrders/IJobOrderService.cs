using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Services.JobOrders
{
    public partial interface IJobOrderService
    {
        #region CRUD

        void InsertJobOrder(JobOrder jobOrder);
        void UpdateJobOrder(JobOrder jobOrder);
        #endregion

        #region JobOrder

        JobOrder GetJobOrderById(int id);

        JobOrder GetJobOrderByGuid(Guid? guid);

        JobOrder CopyJobOrder(JobOrder jobOrder, Guid? origJobOrderGuid);

        JobOrder GetJobOrderByJobPostingIdAndVendorId(int jobPostingId, int vendorId);

        bool JobOrderHasWorkTimeInDateRange(int jobOrderId, DateTime from, DateTime to);
        bool PipelineHasDataInDateRange(int jobOrderId, DateTime from, DateTime to);

        void UpdatePipeLineDateRange(int jobOrderId, DateTime to);

        #endregion

        #region PagedList

        // Web helper methods
        //----------------------------------
        IList<JobOrder> GetLastPublishedJobOrders(int jobOrderCategoryId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        IPagedList<JobOrder> GetAllPublishedJobOrders(string searchString = null, int jobOrderCategoryId = 0, int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);


        // admin helper methods
        //----------------------------------
        IPagedList<JobOrder> GetAllJobOrders(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);


        IQueryable<JobOrder> GetAllJobOrdersAsQueryable(Account account);

        IQueryable<JobOrder> GetAllJobOrdersByCompanyIdAsQueryable(int companyId, bool showHidden = false);

        IQueryable<JobOrder> GetAllJobOrdersByJobPostingIdAsQueryable(Account account, int jobPostingId);

        IList<JobOrder> GetJobOrdersByAccountAndCompany(Account account, int companyId, DateTime refDate);

        IQueryable<JobOrderWithCompanyAddress> GetAllJobOrdersWithCompanyAddressAsQueryable(Account account);

        IList<AttendancePreview> GetAttendancePreviewByDate(DateTime start, DateTime end, int? companyId);
        // Time sheet helper methods
        //----------------------------------

        IQueryable<JobOrder> GetJobOrdersByDateRangeAsQueryable(DateTime startDate, DateTime endDate,bool includeDirectHire = false);
        IQueryable<JobOrder> GetJobOrdersForWorkTimeCalculation(DateTime startDate, DateTime endDate);


        // for missing punch alert (check by start time only!!!)
        IList<JobOrder> GetJobOrdersByDateAndTimeRange(DateTime startDateTime, DateTime endDateTime, int defaultGracePeriod);


        // Client only methods
        //----------------------------------
        IQueryable<JobOrder> GetAllJobOrdersByCompanyIdAsQueryable(Account account, bool showInactive = false, bool showHidden = false);

        IList<JobOrder> GetJobOrdersStartingSoon(DateTime refTime, int min);

        #endregion

        #region JobOrder Search
        IList<JobOrder> SearchJobOrders(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false);
        IList<JobOrder> SearchCompanyJobOrders(Account account, string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false);
        #endregion

        #region Change job order status

        void ChangeJobOrderStatus(int jobOrderId, JobOrderStatusEnum status);

        #endregion


        #region Dynamic Job Order

        IQueryable<JobOrderOpening> GetAllJobOrderOpeningsByDate(Account account, DateTime? refDate = null);

        IQueryable<JobOrderOpening> GetAllJobOrderOpenningsByJobOrder(Guid guid);

        IEnumerable<JobOrderOpening> GetJobOrderOpeningsByDate(Account account, Guid guid, DateTime startDate, DateTime endDate);

        int GetJobOrderOpeningAvailable(int jobOrderId, DateTime inquiryDate, out JobOrderOpening[] openingChanges);

        void UpdateJobOrderOpenings(int jobOrderId, DateTime inquiryDate, int openingAvailable,string note);

        void UpdateJobOrderOpeningForSelectedDate(int jobOrderId, DateTime refDate, int delta, string note);

        string UpdateJobOrderOpenings(int jobOrderId, DateTime startDate, DateTime endDate, int openingNumber, string note);

        string RemoveJobOrderOpening(int openingId);

        #endregion


        #region Check Billing Rates

        IList<int> DoAllJobOrdersHaveBillingRates(List<int> ids, DateTime startDate, DateTime endDate);

        bool AreBillingRatesDefinedForJobOrderByDateRange(int jobOrderId, DateTime startDate, DateTime endDate);

        List<CompanyBillingRate> GetBillingRatesForJobOrderByDateRange(int jobOrderId, DateTime startDate, DateTime endDate);

        #endregion


        #region Modify All job orders from one account to another account
        void MoveAllJobOrdersToNewAccount(int companyId, int newAccountId, int prevAccountId);
        #endregion

    }
}

