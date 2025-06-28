using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Data;
using Wfm.Services.ClockTime;
using Wfm.Services.Configuration;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;


namespace Wfm.Services.Candidates
{
    public partial class CanadiateJobOrderService : ICandidateJobOrderService
    {
        #region Constants

        private const int INVALID = -1;
        private const int NO_CHANGE = 0;
        private const int LATER_START = 1;
        private const int EARLIER_END = 10;
        private const int SHORTER = 100;
        private const int EARLIER_START = 100;
        private const int LATER_END = 1000;
        private const int LONGER = 10000;
        private const int NEW_STATUS = 10000;
        private const int NEW_JOBORDER = 100000;
        private const int NEW_PLACEMENT = 1000000;

        #endregion

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IRepository<CandidateJobOrder> _candidateJobOrderRepository;
        private readonly IRepository<Wfm.Core.Domain.JobPosting.JobPosting> _jobPostingRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<JobOrderOpening> _jobOrderOpeningRepository;
        private readonly IJobOrderTestCategoryService _jobOrderTestCategoryService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateTestResultService _candidateTestResultService;
        private readonly IAttachmentService _attachmentService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IRepository<CandidateWorkTime> _workTimeRepository;
        private readonly IEmployeeTimeChartHistoryService _employeeTimeChartHistoryService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IRepository<CompanyDepartment> _companyDepartmentRepository;
        private readonly IRepository<CandidateClockTime> _candidateClockTimeRepository;
        private readonly IRepository<Franchise> _franchiseRepository;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly ISmartCardService _smartCardService;
        private readonly IHandTemplateService _handTemplateService;
        private readonly IStateProvinceService _stateProvinceService;
        private object smartCardUid;

        #endregion

        #region Ctor

        public CanadiateJobOrderService(
            IWorkContext workContext,
            ISettingService settingService,
            ICandidateService candidateService,
            ICandidateTestResultService candidateTestResultService,
            IAttachmentService attachmentService,
            ICompanyCandidateService companyCandidateService,
            IRepository<Wfm.Core.Domain.JobPosting.JobPosting> jobPostingRepository,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<JobOrderOpening> jobOrderOpeningRepository,
            IJobOrderTestCategoryService jobOrderTestCategoryService,
            IRepository<CandidateJobOrder> candidateJobOrderRepository,
            IRepository<CandidateWorkTime> workTimeRepository,
            IEmployeeTimeChartHistoryService employeeTimeChartHistoryService,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            ILogger logger,
            IDbContext dbContext,
            IRepository<CompanyDepartment> companyDepartmentRepository,
            IRepository<CompanyLocation> companyLocationRepository,
            IRepository<CandidateClockTime> candidateClockTimeRepository,
            IRepository<Franchise> franchiseRepository,
            IClockDeviceService clockDeviceService,
            ISmartCardService smartCardService,
            IHandTemplateService handTemplateService,
            IStateProvinceService stateProvinceService
            ) 
        {
            _workContext = workContext;
            _settingService = settingService;
            _candidateService = candidateService; 
            _candidateTestResultService = candidateTestResultService;
            _attachmentService = attachmentService;
            _companyCandidateService = companyCandidateService;
            _jobPostingRepository = jobPostingRepository;
            _jobOrderRepository = jobOrderRepository;
            _jobOrderOpeningRepository = jobOrderOpeningRepository;
            _jobOrderTestCategoryService = jobOrderTestCategoryService;
            _candidateJobOrderRepository = candidateJobOrderRepository;
            _workTimeRepository = workTimeRepository;
            _employeeTimeChartHistoryService = employeeTimeChartHistoryService;
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _logger = logger;
            _dbContext = dbContext;
            _companyDepartmentRepository = companyDepartmentRepository;
            _companyLocationRepository = companyLocationRepository;
            _candidateClockTimeRepository = candidateClockTimeRepository;
            _franchiseRepository = franchiseRepository;
            _clockDeviceService = clockDeviceService;
            _smartCardService = smartCardService;
            _handTemplateService = handTemplateService;
            _stateProvinceService = stateProvinceService;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Inserts the candidate JobOrder.
        /// </summary>
        /// <param name="candidtateJobOrder">The candidtate job order.</param>
        /// <exception cref="System.ArgumentNullException">candidtateJobOrder</exception>
        public void InsertCandidateJobOrder(CandidateJobOrder candidtateJobOrder, bool logging)
        {
            if (candidtateJobOrder == null)
                throw new ArgumentNullException("candidtateJobOrder");

            _candidateJobOrderRepository.Insert(candidtateJobOrder);

            if (logging)
                _activityLogService.InsertActivityLog("AddCandidateToJobOrderPipeline", _localizationService.GetResource("ActivityLog.AddCandidateToJobOrderPipeline"), _GetLogMsg(candidtateJobOrder));
        }

        /// <summary>
        /// Updates the candidate JobOrder.
        /// </summary>
        /// <param name="candidtateJobOrder">The candidtate job order.</param>
        /// <exception cref="System.ArgumentNullException">candidtateJobOrder</exception>
        public void UpdateCandidateJobOrder(CandidateJobOrder candidtateJobOrder, bool logging = true)
        {
            if (candidtateJobOrder == null)
                throw new ArgumentNullException("candidtateJobOrder");

            _candidateJobOrderRepository.Update(candidtateJobOrder);

            if (logging)
                _activityLogService.InsertActivityLog("UpdateJobOrderPipelineStatus", _localizationService.GetResource("ActivityLog.UpdateJobOrderPipelineStatus"), _GetLogMsg(candidtateJobOrder));
        }

        public void DeleteCandidateJobOrder(CandidateJobOrder candidateJobOrder, bool logging = true)
        {
            if (candidateJobOrder == null)
                throw new ArgumentNullException("candidateJobOrder");

            _candidateJobOrderRepository.Delete(candidateJobOrder);

            if (logging)
                _activityLogService.InsertActivityLog("RemoveCandidateFromJobOrderPipeline", _localizationService.GetResource("ActivityLog.RemoveCandidateFromJobOrderPipeline"), _GetLogMsg(candidateJobOrder));
        }


        public void InsertOrUpdateCandidateJobOrder(int jobOrderId, int candidateId, DateTime startDate, int statusId, DateTime? endDate, int enteredBy, bool logging = true)
        {
            var jo = _jobOrderRepository.Table.Where(x => x.Id == jobOrderId).FirstOrDefault();
            if (jo == null || (jo.EndDate.HasValue && startDate > jo.EndDate)) return;

            startDate = startDate >= jo.StartDate ? startDate : jo.StartDate;
            endDate = endDate.HasValue && (!jo.EndDate.HasValue || endDate < jo.EndDate) ? endDate : jo.EndDate;

            if (_workContext.CurrentAccount != null && enteredBy == 0)
                enteredBy = _workContext.CurrentAccount.Id;

            // all existing
            var canJobOrderList = this.GetAllCandidateJobOrdersByJobOrderIdAndCandidateId(jobOrderId, candidateId);

            // all in range: REMOVE
            var duplicates = canJobOrderList.Where(x => x.StartDate > startDate && ((!x.EndDate.HasValue && !endDate.HasValue) || 
                                                                                    (x.EndDate.HasValue  && (!endDate.HasValue || x.EndDate <= endDate)))).ToList();
            foreach (var d in duplicates)
            {
                var error = RemovePlacement(d);
                if (String.IsNullOrWhiteSpace(error))
                    canJobOrderList.Remove(d);
                else
                    break;
            }

            // any on left end
            var cjo = canJobOrderList.Where(x => x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate >= startDate)).FirstOrDefault();
            // any on right end
            var nextCjo = canJobOrderList.Where(x => x.StartDate > startDate && (!endDate.HasValue || x.StartDate <= endDate)).FirstOrDefault();

            if (nextCjo != null)
            {
                endDate = nextCjo.StartDate.AddDays(-1);

                // forcedly set EndDate for current
                if (cjo != null)
                    cjo.EndDate = endDate;
            }

            // new CandidateJobOrder
            if (cjo == null)
            {
                cjo = new CandidateJobOrder()
                {
                    CandidateId = candidateId,
                    JobOrderId = jobOrderId,
                    StartDate = startDate,
                    EndDate = endDate,
                    CandidateJobOrderStatusId = statusId,
                    EnteredBy = enteredBy, //_workContext.CurrentAccount.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                this.InsertCandidateJobOrder(cjo, logging);
            }

            // existing CandidateJobOrder
            else
            {
                // too short, to be extended
                if (cjo.EndDate.HasValue && (!endDate.HasValue || cjo.EndDate < endDate) && cjo.CandidateJobOrderStatusId == statusId)
                {
                    cjo.EndDate = endDate;
                   // cjo.CandidateJobOrderStatusId = statusId;
                }

                // long enough
                else
                {
                    // no need to update
                    //if (cjo.CandidateJobOrderStatusId == statusId)
                    //    return;

                    // exact match, update status only
                    if (startDate == cjo.StartDate && ((endDate.HasValue && cjo.EndDate.HasValue && endDate == cjo.EndDate) || (!endDate.HasValue && !cjo.EndDate.HasValue)))
                        cjo.CandidateJobOrderStatusId = statusId;

                    // to be divided
                    else
                    {
                        CandidateJobOrder leftCjo = null, rightCjo = null;
                        var newCjo = new CandidateJobOrder()
                        {
                            CandidateId = candidateId,
                            JobOrderId = jobOrderId,
                            CandidateJobOrderStatusId = cjo.CandidateJobOrderStatusId,
                            EnteredBy = enteredBy, //_workContext.CurrentAccount.Id,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        };

                        // on left end of current CandidateJobOrder
                        if (startDate == cjo.StartDate && endDate.HasValue && (!cjo.EndDate.HasValue || endDate < cjo.EndDate))
                            rightCjo = newCjo;

                        // on right end of current CandidateJobOrder, or both open ended
                        else if (cjo.StartDate < startDate &&
                                    ((endDate.HasValue && cjo.EndDate.HasValue && endDate == cjo.EndDate) || (!endDate.HasValue && !cjo.EndDate.HasValue)))
                            leftCjo = newCjo;

                        // in middle of current CandidateJobOrder
                        else
                        {
                            leftCjo = newCjo;
                            rightCjo = newCjo;
                        }

                        if (leftCjo != null)
                        {
                            leftCjo.StartDate = cjo.StartDate;
                            leftCjo.EndDate = startDate.AddDays(-1);
                            this.InsertCandidateJobOrder(leftCjo, logging);
                        }

                        if (rightCjo != null)
                        {
                            rightCjo.StartDate = endDate.Value.AddDays(1);
                            if (nextCjo != null)
                                rightCjo.EndDate = nextCjo.StartDate.AddDays(-1);
                            else
                                rightCjo.EndDate = cjo.EndDate;
                            this.InsertCandidateJobOrder(rightCjo, logging);
                        }

                        cjo.StartDate = startDate;
                        cjo.EndDate = endDate ?? null;
                        cjo.CandidateJobOrderStatusId = statusId;
                    }

                }

                cjo.UpdatedOnUtc = DateTime.UtcNow;
                this.UpdateCandidateJobOrder(cjo, logging);
            }

        }


        private string _GetLogMsg(CandidateJobOrder cjo)
        {
            var logMsg = String.Concat( cjo.CandidateId , "/" , cjo.JobOrderId , "/" ,
                                        cjo.StartDate.ToString("yyyy-MM-dd") , "/" ,
                                        (cjo.EndDate.HasValue ? cjo.EndDate.Value.ToString("yyyy-MM-dd") : "forever") , "/" ,
                                        ((CandidateJobOrderStatusEnum)cjo.CandidateJobOrderStatusId).ToString() );
            return logMsg;
        }

        #endregion

        #region CandidateJobOrders

        /// <summary>
        /// Gets the candidate JobOrder by candidateJobOrderId.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// CandidateJobOrder
        /// </returns>
        public CandidateJobOrder GetCandidateJobOrderById(int id)
        {
            if (id == 0)
                return null;

            return _candidateJobOrderRepository.Table
                .Where(x => x.Id == id)
                .Include(x => x.CandidateJobOrderStatus)
                .Include(x => x.JobOrder)
                .FirstOrDefault();
        }


        public IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersAsQueryable(DateTime? refDate = null, int statusId = 0)
        {
            var query = _candidateJobOrderRepository.Table;

            if (statusId > 0)
                query = query.Where(x => x.CandidateJobOrderStatusId == statusId);

            if (refDate.HasValue)
                query = query.Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate));

            return query;
        }


        public CandidateJobOrder GetCandidateJobOrderByJobOrderIdAndCandidateId(int jobOrderId, int candidateId)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId && j.CandidateId == candidateId
                    orderby j.CreatedOnUtc descending
                    select j;

            return query.FirstOrDefault();
        }

        public IList<CandidateJobOrder> GetCandidateJobOrderByJobOrderIdAndDateRange(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId &&
                          j.StartDate <= endDate && (!j.EndDate.HasValue || j.EndDate >= startDate) &&
                          j.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                    select j;

            return query.ToList();
        }

        public IList<CandidateJobOrder> GetAllCandidateJobOrdersByJobOrderIdAndCandidateId(int jobOrderId, int candidateId)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId && j.CandidateId == candidateId
                    orderby j.StartDate
                    select j;

            return query.OrderBy(x => x.StartDate).ToList();
        }


        public IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersByJobOrderIdAndCandidateIdAndDate(int jobOrderId, int candidateId, DateTime earliestDate)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId && j.CandidateId == candidateId && (!j.EndDate.HasValue || j.EndDate >= earliestDate)
                    orderby j.StartDate
                    select j;

            return query.Include(x => x.Candidate).Include(x => x.JobOrder);
        }


        public bool IsCandidateInJobOrderPipeline(int jobOrderId, int candidateId)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId && j.CandidateId == candidateId
                    select j;

            return query.Any();
        }

        // if placed on any day of the range
        public bool IsCandidatePlacedInJobOrderWithinDateRange(int jobOrderId, int candidateId, DateTime startDate, DateTime endDate)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from j in query
                    where j.JobOrderId == jobOrderId && j.CandidateId == candidateId && j.StartDate <= endDate &&
                            (!j.EndDate.HasValue || j.EndDate >= startDate) &&
                            j.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                    select j;

            return query.Any();
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets the candidate Joborder by JobOrderId.
        /// </summary>
        /// <param name="jobOrderId">The jobOrderId.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns>
        /// IPagedList CandidateJobOrder
        /// </returns>
        public IPagedList<CandidateJobOrder> GetCandidateJobOrderByJobOrderId(int jobOrderId, DateTime? refDate = null,
            int pageIndex = 0, int PageSize = int.MaxValue)
        {
            if (jobOrderId == 0)
                return null;

            var query = _candidateJobOrderRepository.Table;

            query = from c in query
                    where c.JobOrderId == jobOrderId
                    orderby c.UpdatedOnUtc descending
                    select c;
            if (refDate.HasValue)
            {
                query = query.Where(x => x.StartDate <= refDate.Value && (!x.EndDate.HasValue || x.EndDate >= refDate));
            }
            query = query.Include(x => x.JobOrder).Include(x => x.CandidateJobOrderStatus);

            var candidateJobOrders = new PagedList<CandidateJobOrder>(query, pageIndex, PageSize);

            return candidateJobOrders;
        }


        /// <summary>
        /// Gets the CandidateJobOrder by JobOrderId and StatusId.
        /// </summary>
        /// <param name="jobOrderId">The JobOrderId.</param>
        /// <param name="statusId">The JobOrder Status Id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        public IPagedList<CandidateJobOrder> GetCandidateJobOrderByJobOrderIdAndStatusId(int jobOrderId, int statusId, int pageIndex = 0, int PageSize = int.MaxValue)
        {
            if (jobOrderId == 0)
                return null;

            var query = _candidateJobOrderRepository.Table;

            query = from c in query
                    where c.JobOrderId == jobOrderId && c.CandidateJobOrderStatusId == statusId
                    orderby c.UpdatedOnUtc descending
                    select c;

            var candidateJobOrders = new PagedList<CandidateJobOrder>(query, pageIndex, PageSize);

            return candidateJobOrders;
        }


        /// <summary>
        /// Gets the candidate job order by CandidateId.
        /// </summary>
        /// <param name="candidateId">The CandidateId.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        public IPagedList<CandidateJobOrder> GetCandidateJobOrderByCandidateId(int candidateId = 0, int pageIndex = 0, int PageSize = int.MaxValue)
        {
            if (candidateId == 0)
                return null;

            var query = _candidateJobOrderRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId
                    orderby c.UpdatedOnUtc descending
                    select c;

            var candidateJobOrders = new PagedList<CandidateJobOrder>(query, pageIndex, PageSize);

            return candidateJobOrders;
        }



        /// <summary>
        /// Gets the candidate JobOrder by CandidateId AND StatusId.
        /// </summary>
        /// <param name="candidateId">The candidate id.</param>
        /// <param name="statusId">The status id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IPagedList<CandidateJobOrder> GetCandidateJobOrderByCandidateIdAndStatusId(int candidateId, int statusId, int pageIndex = 0, int PageSize = int.MaxValue)
        {
            if (candidateId == 0)
                return null;

            var query = _candidateJobOrderRepository.Table;


            query = from c in query
                    where c.CandidateId == candidateId && c.CandidateJobOrderStatusId == statusId
                    orderby c.UpdatedOnUtc descending
                    select c;

            var candidateJobOrders = new PagedList<CandidateJobOrder>(query, pageIndex, PageSize);

            return candidateJobOrders;
        }


        public IQueryable<CandidateJobOrder> GetCandidateJobOrderByCandidateIdAsQueryable(int candidateId)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId
                    orderby c.CreatedOnUtc descending
                    select c;


            return query.AsQueryable();
        }


        public IQueryable<CandidateJobOrder> GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(int companyId, DateTime refDate)
        {
            var query = _candidateJobOrderRepository.Table;

            query = from c in query
                    where c.JobOrder.CompanyId == companyId &&
                        c.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                        c.StartDate <= refDate && (!c.EndDate.HasValue || c.EndDate.Value >= refDate)
                    select c;

            return query.AsQueryable();
        }


        public IQueryable<CandidateJobOrder> GetLastCandidateJobOrdersBeforeDateAsQueryable(DateTime refDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed)
        {
            var query = _candidateJobOrderRepository.Table;

            var lastDates = query.Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed && x.StartDate < refDate)
                            .GroupBy(grp => grp.CandidateId)
                            .Select(x => new
                            {
                                CandidateId = x.Key,
                                LastStartDate = x.Max(y => y.StartDate)
                            });

            query = from c in query
                    join d in lastDates
                    on new { x = c.CandidateId, y = c.StartDate } equals new { x = d.CandidateId, y = d.LastStartDate }
                    where c.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                    select c;

            return query;
        }


        public IQueryable<CandidateJobOrder> GetFirstCandidateJobOrdersAfterDateAsQueryable(DateTime refDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed)
        {
            var query = _candidateJobOrderRepository.Table;

            var firstDates = query.Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                             (x.StartDate == refDate || (x.StartDate < refDate && (!x.EndDate.HasValue || x.EndDate >= refDate))))
                            .GroupBy(grp => grp.CandidateId)
                            .Select(x => new
                            {
                                CandidateId = x.Key,
                                FirstStartDate = x.Min(y => y.StartDate)
                            });

            query = from c in query
                    join d in firstDates
                    on new { x = c.CandidateId, y = c.StartDate } equals new { x = d.CandidateId, y = d.FirstStartDate }
                    where c.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                    select c;

            return query;
        }


        public IQueryable<CandidateJobOrder> GetAllCandidateJobOrdersByDateRangeAsQueryable(DateTime startDate, DateTime? endDate, int statusId = (int)CandidateJobOrderStatusEnum.Placed)
        {
            var query = _candidateJobOrderRepository.Table;

            query = query.Where(x => x.CandidateJobOrderStatusId == statusId &&
                                     (!endDate.HasValue || x.StartDate <= endDate) && (!x.EndDate.HasValue || x.EndDate >= startDate));

            return query;
        }


        public IQueryable<CandidateAvailableDays> GetAvailableDaysOfCandidateWithinDataRange(DateTime startDate, DateTime endDate)
        {
            var totalDays = (endDate - startDate).Days + 1;
            var result = GetAllCandidateJobOrdersByDateRangeAsQueryable(startDate, endDate)
                         .GroupBy(grp => grp.CandidateId)
                         .Select(x => new CandidateAvailableDays()
                         {
                             CandidateId = x.Key,
                             AvailableDays = totalDays - x.Sum(y => DbFunctions.DiffDays(y.StartDate < startDate ? startDate : y.StartDate, 
                                                                                         !y.EndDate.HasValue || y.EndDate > endDate ? endDate : y.EndDate.Value
                                                                                        ).Value + 1)
                         });

            return result;
        }


        public void UpdateCandidateJobOrderRatingValue(int candidateJobOrderId, decimal rating)
        {
            var entry = _candidateJobOrderRepository.GetById(candidateJobOrderId);
            if (entry != null)
            {
                entry.RatingValue = (int)(Math.Round(rating, 0));
                entry.RatedBy = _workContext.CurrentAccount.FirstName + ' ' + _workContext.CurrentAccount.LastName;
            }
            this.UpdateCandidateJobOrder(entry);
        }


        public IEnumerable<Tuple<DateTime, bool>> GetJobOrderDailyFlags(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var result = Enumerable.Empty<Tuple<DateTime, bool>>();
            if (jobOrderId <= 0 || startDate == null || endDate == null)
                return result;

            var jobOrder = _jobOrderRepository.GetById(jobOrderId);
            if (jobOrder == null)
                return result;

            var holidays = Enumerable.Empty<DateTime>();
            var location = _companyLocationRepository.GetById(jobOrder.CompanyLocationId);
            if (location != null)
                holidays = _stateProvinceService.GetAllStatutoryHolidaysOfStateProvince(location.StateProvinceId)
                    .Where(x => x.HolidayDate >= startDate && x.HolidayDate <= endDate).Select(x => x.HolidayDate).AsEnumerable();

            var dates = Enumerable.Range(0, (endDate - startDate).Days + 1).Select(x => startDate.AddDays(x));
            result = dates.Select(x => Tuple.Create(x, jobOrder.DailySwitches[(int)x.DayOfWeek] && (jobOrder.IncludeHolidays || !holidays.Contains(x))));

            return result;
        }


        public IEnumerable<CandidateJobOrder> GetDailyPlacement(IEnumerable<CandidateJobOrder> placement, DateTime startDate, DateTime endDate)
        {
            var range = endDate != null ? (endDate - startDate).Days + 1 : 7;
            var dates = Enumerable.Range(0, range).Select(x => startDate.AddDays(x));
            var result = from p in placement
                         from d in dates.Where(d => d >= p.StartDate && (!p.EndDate.HasValue || d <= p.EndDate))
                         select new CandidateJobOrder()
                         {
                             CandidateId = p.CandidateId,
                             JobOrderId = p.JobOrderId,
                             CandidateJobOrderStatusId = p.CandidateJobOrderStatusId,
                             StartDate = d,
                             EndDate = d,
                             // virtual entities
                             Candidate = p.Candidate,
                             JobOrder = p.JobOrder,
                         };

            return result;
        }


        public IEnumerable<CandidateJobOrder> GetActualDailyPlacement(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var placement = GetAllCandidateJobOrdersByDateRangeAsQueryable(startDate, endDate).Where(x => x.JobOrderId == jobOrderId);
            var dailyPlacemnt = GetDailyPlacement(placement, startDate, endDate);
            var dailyFlags = GetJobOrderDailyFlags(jobOrderId, startDate, endDate);

            // exclude non work days
            var result = from p in dailyPlacemnt
                         join f in dailyFlags on p.StartDate equals f.Item1
                         where f.Item2
                         select p;

            return result;
        }

        #endregion


        #region Dynamic Job Order

        /// <summary>
        /// Checks if candidate is already placed in other job order.
        /// </summary>
        /// <param name="jobOrderId"></param>
        /// <param name="candidateId"></param>
        /// <param name="startDate"></param>
        /// <returns>return true if candidate is already placed in other job order.</returns>
        public bool IsCandidateAlreadyPlacedInOtherJobOrderWithinDateRange(int candidateId, DateTime startDate, DateTime? endDate = null, int oldJobOrderId = 0)
        {
            var result = _candidateJobOrderRepository.Table
                         .Where(x => x.JobOrderId != oldJobOrderId &&
                                     x.CandidateId == candidateId &&
                                     x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                     (!endDate.HasValue || x.StartDate <= endDate) &&
                                     (!x.EndDate.HasValue || x.EndDate >= startDate)
                               ).Any();

            return result;
        }


        /// <summary>
        /// Get Candidate Job order history by candidate Id as queryable.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        public IQueryable<CandidateJobHistory> GetCandidateJobOrderHistoryByCandidateIdAsQueryable(int candidateId)
        {
            var query = _candidateJobOrderRepository.TableNoTracking;
            var result = from c in query
                         where c.CandidateId == candidateId
                         && c.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                         orderby c.CreatedOnUtc descending
                         select new CandidateJobHistory()
                         {
                             CandidateId = c.CandidateId,
                             JobOrderId = c.JobOrderId,
                             JobOrderGuid = c.JobOrder.JobOrderGuid,
                             CandidateJobOrderStatusId = c.CandidateJobOrderStatusId,
                             RatingValue = c.RatingValue,
                             RatingComment = c.RatingComment,
                             HelpfulYesTotal = c.HelpfulYesTotal,
                             RatedBy = c.RatedBy,
                             Note = c.Note,
                             StartDate = c.StartDate,
                             EndDate = c.EndDate,
                             JobOrderStartDate = c.JobOrder.StartDate,
                             JobOrderEndDate = c.JobOrder.EndDate,
                             JobTitle = c.JobOrder.JobTitle,
                             CompanyName = c.JobOrder.Company.CompanyName,
                             JobOrderStatusName = c.CandidateJobOrderStatus.StatusName,
                             UpdatedOnUtc = c.UpdatedOnUtc,
                             CreatedOnUtc = c.CreatedOnUtc
                         };
            return result.AsQueryable();
        }


        public void SetEndDateAfterClosingJobOrder(int jobOrderId, DateTime endDate)
        {
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("JobOrderId", jobOrderId);
            paras[1] = new SqlParameter("EndDate", endDate);
            const string query = @"Update CandidateJobOrder
                                    Set EndDate = @EndDate
                                    where JobOrderId = @JobOrderId and CandidateJobOrderStatusId=12 and (EndDate is null or EndDate>=@EndDate)";
            _dbContext.ExecuteSqlCommand(query, false, null, paras);

        }


        /// <summary>
        /// Method to return timesheet and payment history by candidate id 
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="paymentHistory"></param>
        /// <returns></returns>
        //public IEnumerable<EmployeeTimeChartHistory> GetTimesheetAndPaymentByCandidateId(int candidateId, out IEnumerable<Candidate_Payment_History> paymentHistory)
        //{
        //    var timeSheets = _employeeTimeChartHistoryService.GetAllEmployeeTimeSheetHistoryByCandidateId(candidateId).ToArray();

        //    var payHistory = _candidatePaymentHistoryRepository.Table
        //           .Where(x => x.CandidateId == candidateId)
        //           .Include(x => x.Candidate_Payment_History_Detail.Select(y => y.Payroll_Item))
        //           .ToArray();

        //    paymentHistory = payHistory;

        //    return timeSheets;
        //}


        /// <summary>
        ///  Method to return timesheet by candidate id and job order id.
        /// </summary>
        /// <param name="candidateId"></param>
        /// <param name="jobOrderId"></param>
        /// <returns></returns>
        public IEnumerable<EmployeeTimeChartHistory> GetCandidateTimeSheetByCandidateIdAndJobOrderId(int candidateId, int jobOrderId)
        {
            var jobOrder = _jobOrderRepository.Table.Where(x => x.Id == jobOrderId).FirstOrDefault();
            var timeSheets = _employeeTimeChartHistoryService.GetAllEmployeeTimeSheetHistoryByCandidateId(candidateId, jobOrder.StartDate, jobOrder.EndDate)
                             .Where(x => x.JobOrderId == jobOrderId)
                             .OrderByDescending(x => x.Year).ThenByDescending(x => x.WeekOfYear)
                             .ToArray();

            return timeSheets;
        }


        /// <summary>
        /// method to return payment history by candidateid. 
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        //public IEnumerable<Candidate_Payment_History> GetCandidate_Payment_HistoryByCandidateId(int candidateId)
        //{

        //    var payHistory = _candidatePaymentHistoryRepository.Table
        //           .Where(x => x.CandidateId == candidateId)
        //           .Include(x => x.Candidate_Payment_History_Detail.Select(y => y.Payroll_Item))
        //           .OrderByDescending(x => x.Payment_Date)
        //           .ToArray();

        //    return payHistory;

        //}

        #endregion


        #region Opening vs Placed

        public int GetTotalCandidateByHired(DateTime datetime)
        {
            int total = (from c in _candidateJobOrderRepository.Table
                         where c.CreatedOnUtc.Value.Day == datetime.Day && c.CandidateJobOrderStatusId == 12
                         select c).Count();
            return total;
        }


        public int GetNumberOfPlacedCandidatesByJobOrder(int jobOrderId, DateTime refDate)
        {
            int total = (from c in _candidateJobOrderRepository.TableNoTracking
                         where c.JobOrderId == jobOrderId &&
                               c.StartDate <= refDate &&
                               (!c.EndDate.HasValue || c.EndDate >= refDate) &&
                               c.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                         select c).Count();

            return total;
        }


        public int GetNumberOfPlacedCandidatesByJobPosting(int jobPostingId, DateTime refDate)
        {
            if (jobPostingId <= 0 || refDate == null)
                return 0;

            var jobOrderIds = _jobOrderRepository.TableNoTracking.Where(x => x.JobPostingId == jobPostingId).Select(x => x.Id);
            var result = _candidateJobOrderRepository.TableNoTracking
                         .Where(x => jobOrderIds.Contains(x.JobOrderId) && x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                         .Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate))
                         .Count();

            return result;
        }


        public bool IsJobAlreadyFilled(int id, DateTime startDate, DateTime? endDate, out DateTime? filledDate, bool byJobOrder, int? refCandidateId = null)
        {
            var firstDate = new DateTime();
            var requested = GetOpeningNumbersPerDay(id, startDate, endDate, out firstDate, byJobOrder);
            var placed = GetPlacedNumbersPerDay(id, startDate, endDate, out firstDate, byJobOrder, refCandidateId);

            filledDate = null;
            for (int i = 0; i < requested.Count; i++)
                if (requested[i] <= placed[i])
                {
                    filledDate = firstDate.AddDays(i);
                    return true;
                }

            return false;
        }


        public string IsJobOrderOrJobPostingAlreadyFilled(int jobOrderId, DateTime startDate, DateTime? endDate, int? refCandidateId = null)
        {
            var result = String.Empty;

            // invalid date range, return non error
            if (endDate.HasValue && endDate.Value < startDate)
                return result;

            var filledDate = new DateTime?();
            if (IsJobAlreadyFilled(jobOrderId, startDate, endDate, out filledDate, true, refCandidateId))
                result = String.Format("Job order is filled since {0} !", filledDate.Value.ToString("yyyy-MM-dd"));
            else
            {
                var jobPostingId = _jobOrderRepository.TableNoTracking.Where(x => x.Id == jobOrderId).FirstOrDefault().JobPostingId;
                if (jobPostingId != null && IsJobAlreadyFilled(jobPostingId.Value, startDate, endDate, out filledDate, false, refCandidateId))
                    result = String.Format("Job posting is filled since {0} !", filledDate.Value.ToString("yyyy-MM-dd"));
            }

            return result;
        }


        public IList<int> GetOpeningNumbersPerDay(int id, DateTime startDate, DateTime? endDate, out DateTime firstDate, bool byJobOrder = true)
        {
            // first valid date in range
            firstDate = startDate;
            IList<int> result = new List<int>();
            if (id <= 0 || startDate == null)
                return result;

            var openings = _jobOrderOpeningRepository.TableNoTracking;
            IList<DateTime> dates = new List<DateTime>();

            if (byJobOrder)
            {
                var jobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
                if (jobOrder != null)
                {
                    openings = openings.Where(x => x.JobOrderId == jobOrder.Id);
                    dates = _GetAllDatesWithinDateRange(jobOrder.StartDate, jobOrder.EndDate, startDate, endDate);
                }
            }
            else
            {
                var jobPosting = _jobPostingRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
                if (jobPosting != null)
                {
                    var jobOrderIds = _jobOrderRepository.TableNoTracking.Where(x => x.JobPostingId == jobPosting.Id).Select(x => x.Id);
                    openings = openings.Where(x => jobOrderIds.Contains(x.JobOrderId));
                    dates = _GetAllDatesWithinDateRange(jobPosting.StartDate, jobPosting.EndDate, startDate, endDate);
                }
            }

            if (dates.Count > 0)
            {
                var dailyOpenings = from d in dates
                                    from o in openings.Where(o => o.StartDate <= d && (!o.EndDate.HasValue || o.EndDate >= d)).DefaultIfEmpty()
                                    select new { Date = d, Opening = o != null ? o.OpeningNumber : 0 };

                if (byJobOrder)
                    result = dailyOpenings.Select(x => x.Opening).ToList();
                else
                {
                    result = (from o in dailyOpenings
                              group new { o.Date, o.Opening } by o.Date into g
                              select new { g.Key, Total = g.Sum(x => x.Opening) })
                             .Select(x => x.Total).ToList();
                }

                firstDate = dates[0];
            }

            return result;
        }


        public IList<int> GetPlacedNumbersPerDay(int id, DateTime startDate, DateTime? endDate, out DateTime firstDate, bool byJobOrder = true, int? refCandidateId = null)
        {
            // first valid date in range
            firstDate = startDate;
            IList<int> result = new List<int>();
            if (id <= 0 || startDate == null)
                return result;

            // TFS5033: if the candidate already placed, the opening check should exclude the candidate
            var placements = _candidateJobOrderRepository.TableNoTracking.Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                .Where(x => !refCandidateId.HasValue || x.CandidateId != refCandidateId.Value);
            IList<DateTime> dates = new List<DateTime>();

            if (byJobOrder)
            {
                var jobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
                if (jobOrder != null)
                {
                    placements = placements.Where(x => x.JobOrderId == jobOrder.Id);
                    dates = _GetAllDatesWithinDateRange(jobOrder.StartDate, jobOrder.EndDate, startDate, endDate);
                }
            }
            else
            {
                var jobPosting = _jobPostingRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
                if (jobPosting != null)
                {
                    var jobOrderIds = _jobOrderRepository.TableNoTracking.Where(x => x.JobPostingId == jobPosting.Id).Select(x => x.Id);
                    placements = placements.Where(x => jobOrderIds.Contains(x.JobOrderId));
                    dates = _GetAllDatesWithinDateRange(jobPosting.StartDate, jobPosting.EndDate, startDate, endDate);
                }
            }

            if (dates.Count > 0)
            {
                var counts = from d in dates
                             from p in placements.Where(p => p.StartDate <= d && (!p.EndDate.HasValue || p.EndDate >= d)).DefaultIfEmpty()
                             group new { d, p } by d into g
                             select new { g.Key, count = g.FirstOrDefault().p == null ? 0 : g.Count() };

                firstDate = dates[0];
                result = counts.Select(x => x.count).ToList();
            }

            return result;
        }


        private IList<DateTime> _GetAllDatesWithinDateRange(DateTime jobStartTime, DateTime? jobEndDate, DateTime startDate, DateTime? endDate)
        {
            // limit date range
            if (startDate < jobStartTime)
                startDate = jobStartTime;
            if (!endDate.HasValue || endDate > startDate.AddDays(29))
                endDate = startDate.AddDays(29);
            if (!jobEndDate.HasValue && endDate > jobEndDate)
                endDate = jobEndDate;

            var dateRange = (endDate.Value - startDate).Days + 1;
            var dates = Enumerable.Range(0, dateRange).Select(offset => startDate.AddDays(offset)).ToList();

            return dates;
        }

        #endregion


        #region validation for placement

        public string IsJobOrderValidForPlacement(int jobOrderId, DateTime startDate, DateTime? endDate)
        {
            var result = String.Empty;
            var jobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == jobOrderId).FirstOrDefault();

            if (jobOrder == null)
                result = "Job order does not exist!";

            if (String.IsNullOrEmpty(result) && jobOrder.IsDeleted || jobOrder.JobOrderStatusId != (int)JobOrderStatusEnum.Active)
                result = "Job order is not active!";

            if (String.IsNullOrEmpty(result) && jobOrder.EndDate.HasValue && jobOrder.EndDate <= startDate)
                result = "Job order is closed!";

            return result;
        }


        public string IsCandidateQualifiedForJobOrder(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate, int? statusId)
        {
            var result = String.Empty;

            var candidate = _candidateService.GetCandidateById(candidateId);
            if (candidate == null)
                result = String.Format("Candidate {0} does not exist!", candidateId);

            // Candidate should be active
            if (String.IsNullOrEmpty(result) && !_candidateService.IsCandidateActive(candidate))
                result = String.Format("Candidate {0} is not active.", candidateId);

            // Candidate should be legally able to work
            if (String.IsNullOrEmpty(result) && !_candidateService.IsCanidateOnboarded(candidate))
                result = String.Format("Candidate {0} is not onboarded yet.", candidateId);

            var jobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == jobOrderId).FirstOrDefault();
            if (jobOrder == null)
                result = "Job order does not exist!";

            // Candidate should be not banned for the company
            if (String.IsNullOrEmpty(result) && _candidateService.IsCandidateBannedByCompanyAndDateRange(candidate.Id, jobOrder.CompanyId, startDate, endDate))
                result = String.Format("Candidate {0} is banned for the company.", candidateId);

            // Candidate and job order should belong to the same franchise
            if (String.IsNullOrEmpty(result) && candidate.FranchiseId != jobOrder.FranchiseId)
                result = String.Format("The candidate {0} cannot be placed in the job order {1} since they have difference vendors.", candidateId, jobOrderId);

            // Candidate has passed the required tests
            if (String.IsNullOrEmpty(result) && !HasCandidatePassedAllTestsRequiredByJobOrder(candidateId, jobOrderId))
                result = String.Format("Candidate {0} has not passed all tests required by Job Order {1}.", candidateId, jobOrderId);

            // must have license if placed to forklift position
            if (jobOrder != null && statusId.HasValue && statusId.Value == (int)CandidateJobOrderStatusEnum.Placed &&
                (jobOrder.Position.Code.ToLower().Contains("forklift") || jobOrder.Position.Name.ToLower().Contains("forklift")))
            {
                var licenseDoc = _attachmentService.GetAttachmentsByCandidateId(candidateId).Where(x => x.DocumentType.InternalCode == "FORKLIFTLICENSE")
                    .OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefault();
                var theEndDate = endDate.HasValue ? endDate.Value : DateTime.MaxValue;
                if (licenseDoc == null || !licenseDoc.ExpiryDate.HasValue)
                    result = String.Format("Candidate {0} does not have a valid forklift license required by Job Order {1}.", candidateId, jobOrderId);
                else if (licenseDoc.ExpiryDate < theEndDate)
                    result = String.Format("Candidate {0} forklift license expires on {1}, while the placement in Job Order {2} ends on {3}.",
                        candidateId, licenseDoc.ExpiryDate.Value.ToShortDateString(), jobOrderId, theEndDate.ToShortDateString());
            }

            if (String.IsNullOrEmpty(result))
            {
                var location = _companyLocationRepository.TableNoTracking.Where(x => x.Id == jobOrder.CompanyLocationId).FirstOrDefault();
                if (location != null)
                {
                    var devices = _clockDeviceService.GetClockDevicesByCompanyLocationId(location.Id);
                    // check smart card, and hand template
                    if (devices.Any() && devices.All(x => !x.AltIdReader))   // no clocks with alt. ID reader
                    {
                        var cards = candidate.SmartCards.Where(x => x.IsActive && !x.IsDeleted).OrderByDescending(x => x.UpdatedOnUtc);
                        if (!cards.Any())
                            result = String.Format("Candidate {0} does not have an active smart card!", candidateId);
                        else if (devices.Any(x => !String.IsNullOrWhiteSpace(x.IPAddress)) && _handTemplateService.GetActiveHandTemplateByCandidateId(candidateId) == null)
                            result = String.Format("Candidate {0} does not have an active hand template!", candidateId);
                        else if (devices.Any(x => x.IDLength == 5))     // clock with 5-digits ID
                        {
                            // check card duplicate
                            var card = cards.First();
                            // TODO: check for whole date range
                            var others = _smartCardService.GetCandidatesByIdString(jobOrder.CompanyId, card.CardNumber.ToString(), out string smartCardUid, true, startDate)
                                .Where(x => x.Id != candidateId);
                            if (others.Any())
                                result = String.Format("Candidate {0} smart card {1} duplicates with others. Please re-issue a new card.", candidateId, card.SmartCardUid);
                        }
                    }
                }

            }
            return result;
        }


        public bool HasCandidatePassedAllTestsRequiredByJobOrder(int candidateId, int jobOrderId)
        {
            var result = false;
            
            if (candidateId > 0 && jobOrderId > 0)
            {
                var testsRequired = _jobOrderTestCategoryService.GetJobOrderTestCategoryByJobOrderId(jobOrderId).Where(x => x.IsActive).Select(x => x.TestCategoryId);
                var testsPassed = _candidateTestResultService.GetCandidateTestResultsByCandidateId(candidateId).Where(x => x.IsPassed).Select(x => x.TestCategoryId);
                result = !testsRequired.Except(testsPassed).Any();
            }

            return result;
        }


        public string AnyApprovedWorkTimeWithinDateRange(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate)
        {
            string result = null;
            string msg = "You cannot change the placement, as some time sheets for candidate {0} in job order {1} from {2} to {3}, have been approved already. " +
                         "Please change the date range and try again.\r\n";

            if (CheckApprovedWorkTimeWithinDateRange(candidateId, jobOrderId, startDate, endDate))
                result = String.Format(msg, candidateId, jobOrderId, startDate.ToString("yyyy-MM-dd"), endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : "forever");

            return result;
        }


        private bool CheckApprovedWorkTimeWithinDateRange(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate = null)
        {
            bool result = _workTimeRepository.Table.Where(x => x.CandidateId == candidateId &&
                                                               x.JobOrderId == jobOrderId &&
                                                               DbFunctions.TruncateTime(x.JobStartDateTime) >= startDate &&
                                                               (!endDate.HasValue || DbFunctions.TruncateTime(x.JobStartDateTime) <= endDate.Value) &&
                                                               x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved
                          ).Any();

            return result;
        }


        public string AnyOtherPlacementWithinDateRange(int newJobOrderId, int candidateId, DateTime startDate, DateTime? endDate, int oldJobOrderId = 0)
        {
            string result = null;

            var jobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == newJobOrderId).FirstOrDefault();
            var errMsg = String.Empty; var hasWarning = false; DateTime? newEndDate = null;

            if (!CheckPlacementForOverlaps(candidateId, newJobOrderId, oldJobOrderId, startDate, endDate, jobOrder.StartTime, jobOrder.EndTime, out errMsg, out hasWarning, out newEndDate))
                result = errMsg;

            //// check duplication (by day), for multi-day placement
            //else
            //{
            //    if (IsCandidateAlreadyPlacedInOtherJobOrderWithinDateRange(candidateId, startDate, endDate, oldJobOrderId))
            //        result = "Candidate already placed in other job orders within this date range. Please change the date range and try again.\r\n";
            //}

            return result;
        }


        public bool _CheckPlacementForOverlaps(int candidateId, int newJobOrderId, int oldJobOrderId,
                                              DateTime fromDate, DateTime? toDate,
                                              DateTime shiftStartTime, DateTime shiftEndTime,
                                              out string Message, out bool HasWarning, out DateTime? EndDate)
        {
            DateTime nextDay = fromDate.AddDays(1);
            DateTime prevDay = fromDate.AddDays(-1);

            shiftStartTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, shiftStartTime.Hour, shiftStartTime.Minute, 0); // overwrite the date portion
            if (shiftEndTime.Hour > shiftStartTime.Hour)
            {
                shiftEndTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, shiftEndTime.Hour, shiftEndTime.Minute, 0); // shifts starts and ends on the same day
            }
            else
            {
                shiftEndTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, shiftEndTime.Hour, shiftEndTime.Minute, 0); // shifts starts and ends on the next day
            }


            bool result = true;
            HasWarning = false;
            Message = String.Empty;
            DateTime availableUntil = DateTime.MaxValue;

            double shiftGap = 8; // Default is 8 hours gap between shifts
            StringBuilder query = new StringBuilder(@"select value from Setting where Name='CandidatePlacement.MinHoursBetweenTwoShifts'");

            try
            {
                var data = _dbContext.SqlQuery<string>(query.ToString());
                shiftGap = Convert.ToDouble(data.ElementAt(0));
            }
            catch (Exception ex)
            {
                // log the error and continue
                _logger.Error("CheckPlacementForOverlaps():", ex);
            }

            //-----------------------------

            query.Clear();
            if (!toDate.HasValue)
            {
                query.AppendLine(@"
                Select cj.JobOrderId, cj.StartDate, cj.EndDate, jo.CompanyId, jo.StartTime, jo.EndTime
                From CandidateJobOrder cj
	                 inner join CandidateJobOrderStatus cjs on cj.CandidateJobOrderStatusId = cjs.Id and cjs.StatusName = 'Placed'
	                 inner join JobOrder jo on jo.Id = cj.JobOrderId and jo.IsDeleted = 0
                Where  cj.candidateId = @candidateId  and cj.JobOrderId not in (@newJobOrderId, @oldJobOrderId)
                       and (cj.EndDate is null or cj.EndDate >= @fromDate or (DateAdd(Day,1,cj.EndDate) = @fromDate and jo.EndTime < jo.StartTime) )
                Order  by CandidateId          
                ");
            }
            else
            {
                query.AppendLine(@"
                Select cj.JobOrderId, cj.StartDate, cj.EndDate, jo.CompanyId, jo.StartTime, jo.EndTime
                From CandidateJobOrder cj
	                 inner join CandidateJobOrderStatus cjs on cj.CandidateJobOrderStatusId = cjs.Id and cjs.StatusName = 'Placed'
	                 inner join JobOrder jo on jo.Id = cj.JobOrderId and jo.IsDeleted = 0
                Where  cj.candidateId = @candidateId  and cj.JobOrderId not in (@newJobOrderId, @oldJobOrderId) and cj.StartDate <= @toDate
                       and (cj.EndDate is null or cj.EndDate >= @fromDate or (DateAdd(Day,1,cj.EndDate) = @fromDate and jo.EndTime < jo.StartTime) )
                Order  by CandidateId          
                ");
            }


            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("candidateId", candidateId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("newJobOrderId", newJobOrderId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("oldJobOrderId", oldJobOrderId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("fromDate", fromDate) { SqlDbType = System.Data.SqlDbType.DateTime });
            if (toDate.HasValue) parameters.Add(new SqlParameter("toDate", toDate.Value) { SqlDbType = System.Data.SqlDbType.DateTime });

            var existingPlacements = _dbContext.SqlQuery<PlacementRecords>(query.ToString(), parameters.ToArray()).ToList<PlacementRecords>();

            DateTime StartTime, EndTime;
            foreach (var row in existingPlacements)
            {
                if (row.EndDate.HasValue && row.EndDate.Value < fromDate)
                    StartTime = new DateTime(prevDay.Year, prevDay.Month, prevDay.Day, row.StartTime.Hour, row.StartTime.Minute, 0);
                else
                    StartTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, row.StartTime.Hour, row.StartTime.Minute, 0);


                if (row.EndTime.Hour > row.StartTime.Hour)
                {
                    EndTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, row.EndTime.Hour, row.EndTime.Minute, 0); // shifts starts and ends on the same day
                }
                else
                {
                    if (row.EndDate.HasValue && row.EndDate.Value < fromDate)
                        EndTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, row.EndTime.Hour, row.EndTime.Minute, 0); // shifts starts and ends on the next day
                    else
                        EndTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, row.EndTime.Hour, row.EndTime.Minute, 0); // shifts starts and ends on the next day
                }

                DateTime startTimeWithGap = StartTime.AddHours(-shiftGap);
                DateTime endTimeWithGap = EndTime.AddHours(shiftGap);

                if (row.StartDate <= fromDate)
                {
                    if (!(shiftStartTime > EndTime || shiftEndTime < StartTime))
                    {
                        result = false; // shifts are overlapping
                        Message = String.Format("Candidate {0} cannot be placed into this job order because he/she is already placed in job order {1} (overlapping placements is not allowed).", candidateId, row.JobOrderId);
                        break;
                    }

                    if ((startTimeWithGap <= shiftStartTime && shiftStartTime < endTimeWithGap) ||
                        (startTimeWithGap < shiftEndTime && shiftEndTime < endTimeWithGap))
                    {
                        result = false; // There is not enough gap between the shifts
                        Message = String.Format("Candidate {0} cannot be placed into this job order because there is not enough gap between the shifts between this job order and job order {1}.", candidateId, row.JobOrderId);
                        break;
                    }
                }
                else
                {
                    if (StartTime <= shiftStartTime || EndTime >= shiftEndTime)
                    {
                        HasWarning = true; // shifts are overlapping
                        if (row.StartDate < availableUntil)
                        {
                            result = false;
                            availableUntil = row.StartDate;
                            Message = String.Format("Candidate {0} cannot be placed into this job order because there will be an overlap with job order {1} starting on {2}.", candidateId, row.JobOrderId, availableUntil.ToShortDateString());
                        }
                        continue;
                    }

                    if ((startTimeWithGap <= shiftStartTime && shiftStartTime < endTimeWithGap) ||
                        (startTimeWithGap < shiftEndTime && shiftEndTime < endTimeWithGap))
                    {
                        HasWarning = true; // There is not enough gap between the shifts
                        if (row.StartDate < availableUntil)
                        {
                            result = false;
                            availableUntil = row.StartDate;
                            Message = String.Format("Candidate {0} cannot be placed into this job order because there will be an overlap with job order {1} starting on {2}.", candidateId, row.JobOrderId, availableUntil.ToShortDateString());
                        }
                        continue;
                    }
                }

            }

            if (availableUntil == DateTime.MaxValue)
                EndDate = null;
            else
                EndDate = availableUntil;

            return result;
        }


        public bool CheckPlacementForOverlaps(int candidateId, int newJobOrderId, int oldJobOrderId,
                                              DateTime fromDate, DateTime? toDate,
                                              DateTime shiftStartTime, DateTime shiftEndTime,
                                              out string Message, out bool HasWarning, out DateTime? EndDate)
        {
            var result = true;
            HasWarning = false;
            Message = String.Empty;
            EndDate = null; // TODO: find availableUntil

            var shiftGap = 8; // Default is 8 hours gap between shifts
            try
            {
                shiftGap = _settingService.GetSettingByKey<int>("CandidatePlacement.MinHoursBetweenTwoShifts");
            }
            catch (Exception ex)
            {
                _logger.Error("CheckPlacementForOverlaps():", ex);
            }

            var addDays = shiftEndTime.TimeOfDay > shiftStartTime.TimeOfDay ? 0 : 1;
            // TODO: check for open ended placement
            var daysToCheck = 14;   // check for 15 days, by default
            var toDateValue = toDate ?? fromDate.AddDays(daysToCheck);
            var dates = Enumerable.Range(0, (toDateValue - fromDate).Days + 1).Select(x => fromDate.AddDays(x).Date);
            var newJobsPlusGap = dates.Select(x => new PlacementRecords()
            {
                JobOrderId = newJobOrderId,
                StartTime = (x + shiftStartTime.TimeOfDay).AddHours(-shiftGap),
                EndTime = (x.AddDays(addDays) + shiftEndTime.TimeOfDay).AddHours(shiftGap),
            });

            var minDate = newJobsPlusGap.First().StartTime.Date;
            var maxDate = newJobsPlusGap.Last().EndTime.Date;
            var existingPlacements = this.GetCandidateJobOrderByCandidateIdAsQueryable(candidateId)
                .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                .Where(x => x.JobOrderId != newJobOrderId && x.JobOrderId != oldJobOrderId)
                .Where(x => x.StartDate <= maxDate && (!x.EndDate.HasValue || x.EndDate >= minDate))
                .Select(x => new PlacementRecords()
                {
                    JobOrderId = x.JobOrderId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CompanyId = x.JobOrder.CompanyId,
                    StartTime = x.JobOrder.StartTime,
                    EndTime = x.JobOrder.EndTime,
                }).OrderBy(x => x.StartDate).ThenBy(x => x.JobOrderId);

            foreach (var p in existingPlacements)
            {
                addDays = p.EndTime.TimeOfDay > p.StartTime.TimeOfDay ? 0 : 1;
                var startDate = p.StartDate > minDate ? p.StartDate : minDate;
                var endDate = p.EndDate.HasValue ? p.EndDate.Value : startDate.AddDays(daysToCheck);
                endDate = endDate < maxDate ? endDate : maxDate;
                var occupiedDates = Enumerable.Range(0, (endDate - startDate).Days + 1).Select(x => startDate.AddDays(x).Date);
                var existingJobs = occupiedDates.Select(x => new PlacementRecords()
                {
                    JobOrderId = p.JobOrderId,
                    StartTime = x + p.StartTime.TimeOfDay,
                    EndTime = x.AddDays(addDays) + p.EndTime.TimeOfDay,
                });
                var overlaps = from e in existingJobs
                               from n in newJobsPlusGap.Where(n => n.EndTime >= e.StartTime && n.StartTime <= e.EndTime).DefaultIfEmpty()
                               where n != null
                               select e;
                if (overlaps.Any())
                {
                    result = false;
                    var overlapDays = overlaps.Select(x => x.StartTime.Date).Distinct().Select(x => x.ToShortDateString());
                    Message = String.Format("Candidate {0} cannot be placed, because there is overlap or no enough gap, between this job order and job order {1}, for days: {2}.", 
                        candidateId, p.JobOrderId, String.Join(", ", overlapDays));
                    break;
                }
            }

            return result;
        }


        public class PlacementRecords
        {
            public int JobOrderId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int CompanyId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        #endregion


        #region Placement

        public string CreateOrSavePlacements(CandidateJobOrder newCjo, bool forecePlacementRulesOnUpdate = false)
        {
            string result = null;

            var existingCjo = GetCandidateJobOrderById(newCjo.Id);
            var changeCode = PlacementChangeCode(existingCjo, newCjo);

            if (changeCode > 0)
            {
                if (changeCode >= NEW_PLACEMENT)
                    result = CreateNewPlacement(newCjo);
                else
                {
                    if (forecePlacementRulesOnUpdate && newCjo.CandidateJobOrderStatusId == (int) CandidateJobOrderStatusEnum.Placed)
                        result = this.ApplyPlacementRules(newCjo);

                    if (String.IsNullOrWhiteSpace(result))
                        result = UpdateExistingPlacement(newCjo);
                }
            }

            return result;
        }

        public string CreateNewPlacement(CandidateJobOrder newCjo, int oldJobOrderId = 0, bool addToCompanyPool = true)
        {
            string result = this.ApplyPlacementRules(newCjo, oldJobOrderId); 

            if (String.IsNullOrEmpty(result))
            {
                int enteredBy = newCjo.EnteredBy;
                if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                    enteredBy = _workContext.CurrentAccount.Id;

                InsertOrUpdateCandidateJobOrder(newCjo.JobOrderId, newCjo.CandidateId, newCjo.StartDate, newCjo.CandidateJobOrderStatusId, newCjo.EndDate, enteredBy);

                #region add to company pool

                if (addToCompanyPool)
                {
                    //  Add candidate to company pool if its not in company pool
                    int[] candidateIds = new int[1];
                    candidateIds[0] = (newCjo.CandidateId);
                    JobOrder jobOrder = _jobOrderRepository.GetById(newCjo.JobOrderId);
                    if (jobOrder != null)
                        _companyCandidateService.AddCandidatesToCompanyIfNotYet(jobOrder.CompanyId, candidateIds, newCjo.StartDate);
                }

                #endregion
            }

            return result;
        }

        public string ApplyPlacementRules(CandidateJobOrder newCjo, int oldJobOrderId = 0)
        {
            string result = null;

            if (newCjo.JobOrder == null)
                newCjo.JobOrder = _jobOrderRepository.TableNoTracking.Where(x => x.Id == newCjo.JobOrderId).FirstOrDefault();

            result = IsJobOrderValidForPlacement(newCjo.JobOrderId, newCjo.StartDate, newCjo.EndDate);

            if (String.IsNullOrEmpty(result))
                result = IsCandidateQualifiedForJobOrder(newCjo.CandidateId, newCjo.JobOrderId, newCjo.StartDate, newCjo.EndDate, newCjo.CandidateJobOrderStatusId);

            if (String.IsNullOrEmpty(result) && newCjo.CandidateJobOrderStatusId == (int) CandidateJobOrderStatusEnum.Placed)
            {
                result = IsJobOrderOrJobPostingAlreadyFilled(newCjo.JobOrderId, newCjo.StartDate, newCjo.EndDate, newCjo.CandidateId);

                if (String.IsNullOrEmpty(result))
                    result = AnyOtherPlacementWithinDateRange(newCjo.JobOrderId, newCjo.CandidateId, newCjo.StartDate, newCjo.EndDate, oldJobOrderId);

                // check if banned for new date range
                if (String.IsNullOrEmpty(result) &&   _candidateService.IsCandidateBannedByCompanyAndDateRange(newCjo.CandidateId, newCjo.JobOrder.CompanyId, newCjo.StartDate, newCjo.EndDate))
                    result = String.Format("Candidate {0} is in DNR list for the company.", newCjo.CandidateId);
            }

            return result;
        }

        public string UpdateExistingPlacement(CandidateJobOrder newCjo, bool keepLeftOver = true)
        {
            string result = null;
            int noStatus = (int)CandidateJobOrderStatusEnum.NoStatus, placed = (int)CandidateJobOrderStatusEnum.Placed;

            var existingCjo = GetCandidateJobOrderById(newCjo.Id);
            var origStartDate = existingCjo.StartDate;
            var origEndDate = existingCjo.EndDate;
            var origStatusId = existingCjo.CandidateJobOrderStatusId;
            var changeCode = PlacementChangeCode(existingCjo, newCjo);

            int enteredBy = newCjo.EnteredBy;
            if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                enteredBy = _workContext.CurrentAccount.Id;

            // to a new job order
            if (changeCode >= NEW_JOBORDER)
            {
                if (existingCjo.CandidateJobOrderStatusId == placed)
                    result += AnyApprovedWorkTimeWithinDateRange(existingCjo.CandidateId, existingCjo.JobOrderId, newCjo.StartDate, newCjo.EndDate);

                if (newCjo.CandidateJobOrderStatusId != placed)
                    result += AnyApprovedWorkTimeWithinDateRange(newCjo.CandidateId, newCjo.JobOrderId, newCjo.StartDate, newCjo.EndDate);

                if (String.IsNullOrWhiteSpace(result))
                {
                    // firstly, try to create the new placement
                    newCjo.Id = 0;
                    result += CreateNewPlacement(newCjo, existingCjo.JobOrderId);

                    if (!String.IsNullOrWhiteSpace(result))
                        return result;

                    // if success, then modify the existing
                    else
                    {
                        DeleteCandidateJobOrder(existingCjo);

                        // re-create left end
                        if ((changeCode % EARLIER_END) >= LATER_START)
                        {
                            var remainStart = origStartDate;
                            var remainEnd = newCjo.StartDate.AddDays(-1);
                            InsertOrUpdateCandidateJobOrder(existingCjo.JobOrderId, existingCjo.CandidateId, remainStart, existingCjo.CandidateJobOrderStatusId, remainEnd, enteredBy);
                        }

                        // re-create right end
                        if (String.IsNullOrWhiteSpace(result) && (changeCode % SHORTER) >= EARLIER_END)
                        {
                            var remainStart = newCjo.EndDate.Value.AddDays(1);
                            var remainEnd = origEndDate;
                            InsertOrUpdateCandidateJobOrder(existingCjo.JobOrderId, existingCjo.CandidateId, remainStart, existingCjo.CandidateJobOrderStatusId, remainEnd, enteredBy);
                        }
                    }
                }
            }

            // in the same job order
            else
            {
                if (newCjo.CandidateJobOrderStatusId == noStatus)
                {
                    if (existingCjo.CandidateJobOrderStatusId == noStatus)
                        DeleteCandidateJobOrder(existingCjo);

                    result += SetCandidateJobOrderToNoStatus(existingCjo.CandidateId, existingCjo.JobOrderId, existingCjo.CandidateJobOrderStatusId, newCjo.StartDate, newCjo.EndDate);
                }

                else
                {
                    // shorter (later start), set remaining to No_Staus
                    if ((changeCode % EARLIER_END) >= LATER_START)
                    {
                        var remainStart = origStartDate;
                        var remainEnd = newCjo.StartDate.AddDays(-1);
                        if (keepLeftOver)
                            result += SetCandidateJobOrderToNoStatus(existingCjo.CandidateId, existingCjo.JobOrderId, existingCjo.CandidateJobOrderStatusId, remainStart, remainEnd);
                        else
                            result += TrimCandidateJobOrderByDateRange(existingCjo, remainStart, remainEnd, leftEnd: true);
                    }

                    // shorter (earlier end), set remaining to No_Status
                    if (String.IsNullOrWhiteSpace(result) && (changeCode % SHORTER) >= EARLIER_END)
                    {
                        var remainStart = newCjo.EndDate.Value.AddDays(1);
                        var remainEnd = origEndDate;
                        if (keepLeftOver)
                            result += SetCandidateJobOrderToNoStatus(existingCjo.CandidateId, existingCjo.JobOrderId, existingCjo.CandidateJobOrderStatusId, remainStart, remainEnd);
                        else
                            result += TrimCandidateJobOrderByDateRange(existingCjo, remainStart, remainEnd, leftEnd: false);
                    }

                    if (String.IsNullOrWhiteSpace(result))
                    {
                        if ( newCjo.CandidateJobOrderStatusId == placed)
                            result += SetCandidateJobOrderToPlaced(existingCjo.CandidateId, existingCjo.JobOrderId, origStartDate, origEndDate, origStatusId, 
                                                                newCjo.StartDate, newCjo.EndDate);
                        else
                            InsertOrUpdateCandidateJobOrder(existingCjo.JobOrderId, existingCjo.CandidateId, newCjo.StartDate, newCjo.CandidateJobOrderStatusId, newCjo.EndDate, enteredBy);
                    }
                }
                

            }

            return result;
        }


        public string SetCandidateJobOrderToStandbyStatus(int candidateId, int jobOrderId, int existingStatusId, 
            DateTime startDate, DateTime? endDate, int newStatusId = (int)CandidateJobOrderStatusEnum.NoStatus)
        {
            string result = null;
            var placed = (int)CandidateJobOrderStatusEnum.Placed;

            if (newStatusId == placed)
                return "Status 'Placed' is not applicable here.";

            if (existingStatusId == placed)
                result = AnyApprovedWorkTimeWithinDateRange(candidateId, jobOrderId, startDate, endDate);

            if (string.IsNullOrEmpty(result))
            {
                int enteredBy = 0;
                if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                    enteredBy = _workContext.CurrentAccount.Id;

                InsertOrUpdateCandidateJobOrder(jobOrderId, candidateId, startDate, newStatusId, endDate, enteredBy);
            }

            return result;
        }


        public string SetCandidateJobOrderToNoStatus(int candidateId, int jobOrderId, int existingStatusId, DateTime startDate, DateTime? endDate)
        {
            return SetCandidateJobOrderToStandbyStatus(candidateId, jobOrderId, existingStatusId, startDate, endDate);
        }


        public string TrimCandidateJobOrderByDateRange(CandidateJobOrder existingCjo, DateTime startDate, DateTime? endDate, bool leftEnd)
        {
            string result = null;
            int placed = (int)CandidateJobOrderStatusEnum.Placed;

            if (existingCjo.CandidateJobOrderStatusId == placed)
                result = AnyApprovedWorkTimeWithinDateRange(existingCjo.CandidateId, existingCjo.JobOrderId, startDate, endDate);

            if (String.IsNullOrEmpty(result))
            {
                if (leftEnd && endDate.HasValue)
                    existingCjo.StartDate = endDate.Value.AddDays(1);
                else
                    existingCjo.EndDate = startDate.AddDays(-1);
                UpdateCandidateJobOrder(existingCjo);
            }

            return result;
        }


        public string SetCandidateJobOrderToPlaced(int candidateId, int jobOrderId, DateTime existingStartDate, DateTime? existingEndDate, int existingStatusId, DateTime startDate, DateTime? endDate)
        {
            string result = null;
            int placed = (int)CandidateJobOrderStatusEnum.Placed;

            // to PLACED
            if (existingStatusId != placed)
                result = IsJobOrderOrJobPostingAlreadyFilled(jobOrderId, startDate, endDate, candidateId);

            // longer
            else
            {
                // left end
                result = IsJobOrderOrJobPostingAlreadyFilled(jobOrderId, startDate, existingStartDate.AddDays(-1), candidateId);
                if (String.IsNullOrWhiteSpace(result) && existingEndDate.HasValue)
                    // right end
                    result = IsJobOrderOrJobPostingAlreadyFilled(jobOrderId, existingEndDate.Value.AddDays(1), endDate, candidateId);
            }

            if (String.IsNullOrEmpty(result))
                result = AnyOtherPlacementWithinDateRange(jobOrderId, candidateId, startDate, endDate, existingStatusId == placed ? jobOrderId : 0);

            if (String.IsNullOrEmpty(result))
                InsertOrUpdateCandidateJobOrder(jobOrderId, candidateId, startDate, placed, endDate, _workContext.CurrentAccount.Id);

            return result;
        }


        public string RemovePlacement(CandidateJobOrder existingCjo)
        {
            string result = null;

            if (existingCjo != null)
            {
                result = AnyApprovedWorkTimeWithinDateRange(existingCjo.CandidateId, existingCjo.JobOrderId, existingCjo.StartDate, existingCjo.EndDate);
                if (String.IsNullOrEmpty(result))
                    DeleteCandidateJobOrder(existingCjo);
            }

            return result;
        }


        private int PlacementChangeCode(CandidateJobOrder existingCjo, CandidateJobOrder newCjo)
        {
            int code = NO_CHANGE;

            if (existingCjo == null)
                code += NEW_PLACEMENT;

            else if (newCjo != null)
            {
                if (newCjo.CandidateJobOrderStatusId != existingCjo.CandidateJobOrderStatusId) code += NEW_STATUS;

                if (newCjo.JobOrderId != existingCjo.JobOrderId) code += NEW_JOBORDER;
                // no overlap, regarded as new job order
                else if ((existingCjo.EndDate.HasValue && newCjo.StartDate > existingCjo.EndDate) || (newCjo.EndDate.HasValue && newCjo.EndDate < existingCjo.StartDate))
                {
                    code += NEW_JOBORDER;
                    return code;
                }

                if (newCjo.EndDate.HasValue)
                {
                    if (existingCjo.EndDate.HasValue && newCjo.EndDate > existingCjo.EndDate) code += LATER_END;
                    if (!existingCjo.EndDate.HasValue || newCjo.EndDate < existingCjo.EndDate) code += EARLIER_END;
                }
                else if (existingCjo.EndDate.HasValue) code += LATER_END;

                if (newCjo.StartDate < existingCjo.StartDate) code += EARLIER_START;
                else if (newCjo.StartDate > existingCjo.StartDate) code += LATER_START;
            }

            return code;
        }

        public string RemoveCandidateFromAllPipelines(int candidateId, DateTime startDate, int? companyId = null)
        {
            var result = new StringBuilder("");

            var placements = _candidateJobOrderRepository.Table
                             .Where(x => x.CandidateId == candidateId)
                             .Where(x => !x.EndDate.HasValue || x.EndDate >= startDate);

            if (companyId.HasValue)
                placements = placements.Where(x => x.JobOrder.CompanyId == companyId);

            foreach (var p in placements.ToList())
            {
                if (p.StartDate >= startDate)
                {
                    var msg = RemovePlacement(p);
                    if (!String.IsNullOrEmpty(msg))
                        result = result.AppendLine(msg);
                }
                else
                {
                    var msg = TrimCandidateJobOrderByDateRange(p, startDate, null, leftEnd: false);
                    if (!String.IsNullOrEmpty(msg))
                        result = result.AppendLine(msg);
                }
            }

            return result.ToString();
        }

        #endregion

        #region Client DailyAttendance List

        public IList<DailyAttendanceList> GetDailyAttendanceList(DateTime? refDate, DateTime? clientDateTime, Account account = null)
        {
            if (account == null)
                return null;

            if (refDate == null)
                refDate = DateTime.Today.Date;

            var query = _candidateJobOrderRepository.Table;
            // query within company
            query = query.Where(cjo => cjo.JobOrder.CompanyId == account.CompanyId);
          
            query = query.Where(cjo => cjo.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cjo => cjo.JobOrder.CompanyLocationId > 0 &&
                                           cjo.JobOrder.CompanyLocationId == account.CompanyLocationId); // search within location

            // Jobs for Department Supervisor and Department Manager
            else if (account.IsCompanyDepartmentSupervisor() || account.IsCompanyDepartmentManager())
            {
                query = query.Where(cjo => cjo.JobOrder.CompanyLocationId > 0 &&
                                           cjo.JobOrder.CompanyLocationId == account.CompanyLocationId &&
                                           cjo.JobOrder.CompanyDepartmentId > 0 &&
                                           cjo.JobOrder.CompanyDepartmentId == account.CompanyDepartmentId  // search within department
                                          );
                if (account.IsCompanyDepartmentSupervisor())
                    query = query.Where(cjo => cjo.JobOrder.CompanyContactId == account.Id);
            }
            else
                return null; // no role

            var totalQuery = _workTimeRepository.TableNoTracking
                                .Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved&&x.CompanyId==account.CompanyId)
                                .Select(x =>new {x.CandidateId,x.NetWorkTimeInHours})
                                .GroupBy(x =>x.CandidateId)
                                .Select(x => new { candidateId=x.Key, total=x.Sum(y=>y.NetWorkTimeInHours) });


            var result = from cjo in query.Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate))
                         join tl in totalQuery on cjo.CandidateId equals tl.candidateId into group0
                         from g0 in group0.DefaultIfEmpty()
                         join cl in _companyLocationRepository.TableNoTracking on cjo.JobOrder.CompanyLocationId equals cl.Id into group1
                         from g1 in group1.DefaultIfEmpty()
                         join cd in _companyDepartmentRepository.TableNoTracking on cjo.JobOrder.CompanyDepartmentId equals cd.Id into group2
                         from g2 in group2.DefaultIfEmpty()
                         join vend in _franchiseRepository.TableNoTracking on cjo.JobOrder.FranchiseId equals vend.Id into group3
                         from g3 in group3.DefaultIfEmpty()
                         join cwt in _workTimeRepository.TableNoTracking.Where(x => DbFunctions.TruncateTime(x.JobStartDateTime) == refDate.Value)
                            .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched) 
                            on new { c = cjo.CandidateId, j = cjo.JobOrderId } equals new { c = cwt.CandidateId, j = cwt.JobOrderId } into group4
                         from g4 in group4.DefaultIfEmpty()
                         orderby cjo.JobOrder.StartTime
                         select new DailyAttendanceList()
                         {
                             CandidateJobOrderId = cjo.Id,
                             CandidateId = cjo.CandidateId,
                             CandidateGuid = cjo.Candidate.CandidateGuid,
                             EmployeeFirstName = cjo.Candidate.FirstName,
                             Department = g2.DepartmentName,
                             EmployeeId = cjo.Candidate.EmployeeId,
                             EmployeeLastName = cjo.Candidate.LastName,
                             JobOrderId = cjo.JobOrder.Id,
                             JobTitle = cjo.JobOrder.JobTitle,
                             Location = g1.LocationName,
                             ShiftStartTime = DbFunctions.CreateDateTime(refDate.Value.Year, refDate.Value.Month, refDate.Value.Day, cjo.JobOrder.StartTime.Hour, cjo.JobOrder.StartTime.Minute, cjo.JobOrder.StartTime.Second).Value,
                             ShiftEndTime = cjo.JobOrder.EndTime,
                             Status = g4 != null ? "Punched in" : DbFunctions.CreateDateTime(refDate.Value.Year, refDate.Value.Month, refDate.Value.Day, cjo.JobOrder.StartTime.Hour, cjo.JobOrder.StartTime.Minute, cjo.JobOrder.StartTime.Second) > clientDateTime ? "Scheduled" : "No Show",
                             JobTitleAndId = cjo.JobOrderId.ToString() + " - " + cjo.JobOrder.JobTitle,
                             VendorName = g3.FranchiseName,
                             TotalHoursWorked = g0!=null?g0.total:0
                         };

            return result.Distinct().ToList();
        }


        public IQueryable<DailyPlacement> GetDailyPlacments(IQueryable<CandidateJobOrder> source, DateTime startDate, DateTime endDate, bool workDayOnly = true)
        {
            var dateRange = (endDate - startDate).Days + 1;
            var dates = Enumerable.Range(0, dateRange).Select(offset => startDate.AddDays(offset));

            var result = from p in source
                         join d in dates on 1 equals 1
                         where d >= p.StartDate && (!p.EndDate.HasValue || d <= p.EndDate)
                            && (!workDayOnly 
                            || ((SqlFunctions.DatePart("dw", d) == 1 && p.JobOrder.SundaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 2 && p.JobOrder.MondaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 3 && p.JobOrder.TuesdaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 4 && p.JobOrder.WednesdaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 5 && p.JobOrder.ThursdaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 6 && p.JobOrder.FridaySwitch)
                            || (SqlFunctions.DatePart("dw", d) == 7 && p.JobOrder.SaturdaySwitch)))
                         select new DailyPlacement()
                         {
                             CandidateId = p.CandidateId,
                             JobOrderId = p.JobOrderId,
                             StartDate = d,
                             EndDate = (DateTime)DbFunctions.AddDays(d, p.JobOrder.EndTime.Hour < p.JobOrder.StartTime.Hour ? 1 : 0),
                             CandidateJobOrderStatusId = p.CandidateJobOrderStatusId,
                             // any other properties
                             Candidate = p.Candidate,
                             JobOrder = p.JobOrder,
                         };

            return result;
        }

        #endregion
    }
}
