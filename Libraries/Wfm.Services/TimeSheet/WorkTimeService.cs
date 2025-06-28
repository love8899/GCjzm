using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Policies;
using Wfm.Data;
using Wfm.Services.ClockTime;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Policies;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;
using Wfm.Services.Logging;
using Wfm.Services.Configuration;
using Wfm.Services.Localization;


namespace Wfm.Services.TimeSheet
{
    public partial class WorkTimeService : IWorkTimeService
    {

        #region Fields

        private readonly IDbContext _dbContext;

        private readonly IWorkContext _workContext;
        private readonly IRepository<CandidateWorkTime> _workTimeRepository;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IRepository<ClientTimeSheetDocument> _clientTimeSheetDocumentRepositoty;
        private readonly IRepository<CandidateWorkOverTime> _candidateWorkOverTimeRepository;
        private readonly IRepository<Alerts> _alertsRepository;
        private readonly IRepository<CompanyDepartment> _companyDepartmentRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;

        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateService _candidateService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyContactService _companyContactService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IRoundingPolicyService _roundingPolicyService;
        private readonly IMealPolicyService _mealPolicyService;
        private readonly IBreakPolicyService _breakPolicyService;
        private readonly ISchedulePolicyService _schedulePolicyService;
        private readonly ISmartCardService _smartCardService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly IClockTimeService _clockTimeService;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ISettingService _settingService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        private const int WORKTIME_DECIMAL_PLACES = 2;

        #endregion

        #region Ctor

        public WorkTimeService(IDbContext dbContext,
            IWorkContext workContext,
            IRepository<CandidateWorkTime> workTimeRepository,
            IRepository<CompanyLocation> companyLocationRepository,
            IRepository<ClientTimeSheetDocument> clientTimeSheetDocumentRepository,
            IRepository<CandidateWorkOverTime> candidateWorkOverTimeRepository,
            IRepository<Alerts> alertsRepository,
            IRepository<CompanyDepartment> companyDepartmentRepository,
            IRepository<JobOrder> jobOrderRepository,

            IActivityLogService activityLogService,
            ICandidateService candidateService,
            ICompanyService companyService,
            ICompanyContactService companyContactService,
            IJobOrderService jobOrderService,
            IRoundingPolicyService roundingPolicyService,
            IMealPolicyService mealPolicyService,
            IBreakPolicyService breakPolicyService,
            ISchedulePolicyService schedulePolicyService,
            ISmartCardService smartCardService,
            IClockDeviceService clockDeviceService,
            IClockTimeService clockTimeService,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            IWorkflowMessageService workflowMessageService,
            IScheduleTaskService scheduleTaskService,
            ICandidateJobOrderService candidateJobOrderService,
            ISettingService settingService,
            IRecruiterCompanyService recruiterCompanyService,
            ILogger logger,
            ILocalizationService localizationService
            )
        {
            _dbContext = dbContext;
            _workContext = workContext;
            _workTimeRepository = workTimeRepository;
            _companyLocationRepository = companyLocationRepository;
            _clientTimeSheetDocumentRepositoty = clientTimeSheetDocumentRepository;
            _candidateWorkOverTimeRepository = candidateWorkOverTimeRepository;
            _alertsRepository = alertsRepository;
            _companyDepartmentRepository = companyDepartmentRepository;
            _jobOrderRepository = jobOrderRepository;

            _activityLogService = activityLogService;
            _candidateService = candidateService;
            _companyService = companyService;
            _companyContactService = companyContactService;
            _jobOrderService = jobOrderService;
            _roundingPolicyService = roundingPolicyService;
            _mealPolicyService = mealPolicyService;
            _breakPolicyService = breakPolicyService;
            _schedulePolicyService = schedulePolicyService;
            _smartCardService = smartCardService;
            _clockDeviceService = clockDeviceService;
            _clockTimeService = clockTimeService;
            _candidateWorkTimeSettings = candidateWorkTimeSettings;
            _workflowMessageService = workflowMessageService;
            _scheduleTaskService = scheduleTaskService;
            _candidateJobOrderService = candidateJobOrderService;
            _settingService = settingService;
            _recruiterCompanyService = recruiterCompanyService;
            _logger = logger;
            _localizationService = localizationService;
        }

        #endregion


        #region CRUD

        public void InsertCandidateWorkTime(CandidateWorkTime candidateWorkTime)
        {
            if (candidateWorkTime == null)
                throw new ArgumentNullException("candidateWorkTime");

            _workTimeRepository.Insert(candidateWorkTime);
        }

        public void UpdateCandidateWorkTime(CandidateWorkTime candidateWorkTime)
        {
            if (candidateWorkTime == null)
                throw new ArgumentNullException("candidateWorkTime");

            if (candidateWorkTime.Payroll_BatchId.HasValue && candidateWorkTime.Payroll_BatchId != 0)
                throw new WfmException("This record is locked by payroll and cannot be modified.");

            _workTimeRepository.Update(candidateWorkTime);
        }

        public void DeleteCandidateWorkTime(CandidateWorkTime candidateWorkTime)
        {
            if (candidateWorkTime == null)
                throw new ArgumentNullException("candidateWorkTime");

            if (candidateWorkTime.Payroll_BatchId.HasValue && candidateWorkTime.Payroll_BatchId != 0)
                throw new WfmException("This record is locked by payroll and cannot be modified.");

            _workTimeRepository.Delete(candidateWorkTime);
        }


        // Insert or Update, for import and manual, as well as missing hour
        public int InsertOrUpdateWorkTime(int jobOrderId, int candidateId, DateTime startDate, decimal hours, string source, string note = null, bool logging = true, bool overwriteOrig = false)
        {
            var candidateWorkTime = this.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, jobOrderId, startDate);
            if (candidateWorkTime != null)
            {
                // skip work time from punch clock, no matter status
                // unless explicit overwrite (pending approval only)
                if (candidateWorkTime.ClockDeviceUid != null && !(overwriteOrig && candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval))
                    return 0;

                // NOT skip appoved manual/import work time, skip voided work time
                if (false ||
                    candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Voided)
                    return 0;

                // mark source
                if (source == null || (source != WorkTimeSource.Manual && source != WorkTimeSource.Import))
                    source = WorkTimeSource.Manual;
                candidateWorkTime.Source = source;

                // in case job order updated
                var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
                if (jobOrder != null && jobOrder.UpdatedOnUtc > candidateWorkTime.UpdatedOnUtc)
                {
                    candidateWorkTime.CompanyId = jobOrder.CompanyId;
                    candidateWorkTime.ShiftId = jobOrder.ShiftId;
                    candidateWorkTime.CompanyLocationId = jobOrder.CompanyLocationId;
                    candidateWorkTime.CompanyDepartmentId = jobOrder.CompanyDepartmentId;
                    candidateWorkTime.CompanyContactId = jobOrder.CompanyContactId;
                    candidateWorkTime.JobStartDateTime = candidateWorkTime.JobStartDateTime.Date + jobOrder.StartTime.TimeOfDay;
                    candidateWorkTime.JobEndDateTime = candidateWorkTime.JobStartDateTime.AddDays(jobOrder.EndTime.TimeOfDay < jobOrder.StartTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay;
                }

                candidateWorkTime.NetWorkTimeInHours = CommonHelper.RoundUp(hours, 2);
                candidateWorkTime.NetWorkTimeInMinutes = hours * 60;
                candidateWorkTime.AdjustmentInMinutes = hours * 60;
                if (note != null)
                    candidateWorkTime.Note = note;

                // save as approved
                candidateWorkTime.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.Approved;
                candidateWorkTime.ApprovedOnUtc = candidateWorkTime.UpdatedOnUtc = DateTime.UtcNow;
                candidateWorkTime.ApprovedBy = _workContext.CurrentAccount.Id;
                candidateWorkTime.ApprovedByName = _workContext.CurrentAccount.FullName;
                this.UpdateCandidateWorkTime(candidateWorkTime);

                // remove other matched work times
                this.RemoveOtherMatchedWorkTimes(candidateWorkTime);
                // update clocktime status
                this.SetClockTimeStatusByWorkTime(candidateWorkTime);

                if (logging)
                    _activityLogService.InsertActivityLog("ApproveWorkTime", String.Format("Candidate ({0}) time sheet on {1} under Job order ({2}) is approved.",
                                                          candidateId, startDate.ToShortDateString(), jobOrderId));
            }
            else
            {
                var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
                candidateWorkTime = new CandidateWorkTime()
                {
                    CandidateId = candidateId,
                    JobOrderId = jobOrderId,
                    JobStartDateTime = startDate.Date + jobOrder.StartTime.TimeOfDay,
                    JobEndDateTime = startDate.AddDays(jobOrder.EndTime.TimeOfDay < jobOrder.StartTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay,
                    NetWorkTimeInHours = CommonHelper.RoundUp(hours, 2),
                    Note = note
                };

                // save as approved
                this.SaveManualCandidateWorkTime(candidateWorkTime, source, (int)CandidateWorkTimeStatus.Approved);

                if (logging)
                    _activityLogService.InsertActivityLog("ApproveWorkTime", String.Format("Candidate ({0}) time sheet on {1} under Job order ({2}) is approved.",
                                                          candidateId, startDate.ToShortDateString(), jobOrderId));
            }

            return candidateWorkTime != null ? candidateWorkTime.Id : 0;
        }


        public void CalculateOTforWorktimeWithinDateRange(DateTime startDate, DateTime endDate)
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("StartDate", startDate);

            _dbContext.ExecuteSqlCommand("EXEC [dbo].[Calculate_Overtime_For_Approved_Hours] @StartDate", false, null, paras);
        }

        #endregion


        #region CandidateWorkTime

        public CandidateWorkTime GetWorkTimeById(int id)
        {
            if (id == 0)
                return null;

            return _workTimeRepository.GetById(id);
        }

        public CandidateWorkTime GetWorkTimeByCandidateIdAndDate(int candidateId, DateTime selectedDate)
        {
            var query = _workTimeRepository.Table;

            query = from c in _workTimeRepository.Table
                    where (c.ClockIn.Value.Year == selectedDate.Year
                        && c.ClockIn.Value.Month == selectedDate.Month
                        && c.ClockIn.Value.Day == selectedDate.Day)
                        && c.CandidateId == candidateId
                        && c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched
                    orderby c.CandidateId, c.ClockIn descending
                    select c;

            return query.FirstOrDefault();
        }

        public CandidateWorkTime GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate, bool includeMatched = false)
        {
            var query = _workTimeRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId &&
                    c.JobOrderId == jobOrderId &&
                    c.JobStartDateTime.Year == jobStartDate.Year &&
                    c.JobStartDateTime.Month == jobStartDate.Month &&
                    c.JobStartDateTime.Day == jobStartDate.Day
                    select c;

            if (!includeMatched)
                query = query.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);

            return query.FirstOrDefault();
        }

        #endregion


        #region LIST

        public IList<CandidateWorkTime> GetWorkTimeByIds(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return new List<CandidateWorkTime>();

            var query = from p in _workTimeRepository.Table
                        where ids.Contains(p.Id)
                        select p;
            var worktimes = query.ToList();

            var sortedworktimes = new List<CandidateWorkTime>();

            foreach (int id in ids)
            {
                var product = worktimes.Find(x => x.Id == id);
                if (product != null)
                    sortedworktimes.Add(product);
            }
            return sortedworktimes;
        }


        public IQueryable<CandidateWorkTime> GetWorkTimeByCandidateIdAsQueryable(int candidateId = 0)
        {
            if (candidateId == 0)
                return null;

            var query = _workTimeRepository.Table;

            query = query.Where(cwt => cwt.CandidateId == candidateId && cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);

            query = from cwt in query
                    orderby cwt.JobStartDateTime descending
                    select cwt;

            return query.AsQueryable();
        }


        public IQueryable<CandidateWorkTime> GetWorkTimeByStartEndDateAsQueryable(DateTime fromDate, DateTime toDate, bool includeMatched = false)
        {
            var query = _workTimeRepository.Table;

            toDate = toDate.AddDays(1);

            query = query.Where(c => c.JobStartDateTime >= fromDate.Date && c.JobStartDateTime < toDate.Date);

            if (!includeMatched)
                query = query.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);

            query = from c in query
                    orderby c.JobEndDateTime // This is critical for overtime calculation, need by time order
                    select c;

            return query.AsQueryable();
        }


        // for invoice
        public IQueryable<CandidateWorkTime> GetWorkTimeNotFullyInvoicedByStartEndDateAsQueryable(DateTime fromDate, DateTime toDate)
        {
            var query = this.GetWorkTimeByStartEndDateAsQueryable(fromDate, toDate);

            query = query.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Rejected);

            query = query.Where(x => (x.InvoiceDate == null && x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved && x.NetWorkTimeInHours > 0) ||
                                     (x.InvoiceDate != null && x.CandidateWorkTimeChanges.OrderByDescending(y => y.CreatedOnUtc).FirstOrDefault().CreatedOnUtc > x.InvoiceDate)
                                );
            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
                query = query.Where(x => x.FranchiseId == account.FranchiseId);
            return query.AsQueryable();
        }


        // for candidate attendance calculation
        public IQueryable<CandidateWorkTime> GetAllWorkTimeByJobOrderAndDateAsQueryable(int jobOrderId, DateTime selectedDate, bool includeVoided = false)
        {
            var query = _workTimeRepository.Table;

            // filter out voided and matched work times
            if (!includeVoided)
                query = query.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided);

            query = query.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);

            // selected job order
            query = query.Where(x => x.JobOrderId == jobOrderId);

            // selected date
            query = query.Where(c => DbFunctions.TruncateTime(c.JobStartDateTime) == DbFunctions.TruncateTime(selectedDate.Date));

            return query.AsQueryable();
        }


        public IQueryable<CandidateWorkTime> GetAllWorkTimeByJobOrderAndStartEndDateAsQueryable(int jobOrderId, DateTime fromDate, DateTime toDate)
        {
            var query = _workTimeRepository.Table;

            // filter out voided and matched work times
            query = query.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided && c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);

            // selected job order
            query = query.Where(x => x.JobOrderId == jobOrderId);

            // selected date
            query = query.Where(c => DbFunctions.TruncateTime(c.JobStartDateTime) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(c.JobStartDateTime) <= DbFunctions.TruncateTime(toDate));

            return query.AsQueryable();
        }


        // For Admin
        public IQueryable<CandidateWorkTime> GetAllCandidateWorkTimeAsQueryable(bool isAccountBased = true)
        {
            var query = _workTimeRepository.TableNoTracking;

            if (isAccountBased)
            {
                List<int> ids = new List<int>();
                Account account = _workContext.CurrentAccount;
                if (account.IsVendor())
                {
                    query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);
                    if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                    {
                        ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        query = query.Where(x => ids.Contains(x.CompanyId));
                    }
                }
                else
                {
                    if (account.IsMSPRecruiterSupervisor() || account.IsMSPRecruiter())
                    {
                        ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        query = query.Where(x => ids.Contains(x.CompanyId));
                    }
                }
            }

            query = from cwt in query
                    // orderby cwt.JobStartDateTime descending // sort by job start date
                    // additioal sort to make paging consistent
                    orderby cwt.JobStartDateTime descending, cwt.JobOrderId descending, cwt.CandidateId
                    select cwt;

            return query.AsQueryable();
        }


        // For Client
        public IQueryable<CandidateWorkTime> GetAllCandidateWorkTimeByAccountAsQueryable(Account account = null, bool showRejected = false)
        {
            if (account == null)
                return null;

            var query = _workTimeRepository.Table;

            // query within company
            query = query.Where(cwt => cwt.CompanyId == account.CompanyId);

            // except Voided, Matched, and Rejected
            query = query.Where(cwt => cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided &&
                                       cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched &&
                                       (showRejected || cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Rejected));

            // net work time is greater than 1
            //query = query.Where(cwt => cwt.NetWorkTimeInHours >= 1);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId); // search within locatin

            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId &&
                    cwt.CompanyDepartmentId > 0 &&
                    cwt.CompanyDepartmentId == account.CompanyDepartmentId &&   // search within department
                    cwt.CompanyContactId == account.Id);
            else if (account.IsCompanyDepartmentManager())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId &&
                    cwt.CompanyDepartmentId > 0 &&
                    cwt.CompanyDepartmentId == account.CompanyDepartmentId);
            else
                return null; // no role

            query = from cwt in query
                    orderby cwt.JobStartDateTime descending // sort by job start date
                    select cwt;

            return query.AsQueryable();
        }

        public IQueryable<EmployeeWorkTimeApproval> GetDailyTimeSheetsByAccountAsQueryable(Account account = null, bool showRejected = false)
        {
            var result = GetAllCandidateWorkTimeByAccountAsQueryable(account, showRejected)
                        .Select(cwt => new EmployeeWorkTimeApproval()
                        {
                            Id = cwt.Id,
                            Year = cwt.Year,
                            WeekOfYear = cwt.WeekOfYear,
                            Payroll_BatchId = cwt.Payroll_BatchId,
                            CandidateId = cwt.CandidateId,
                            CandidateGuid = cwt.Candidate.CandidateGuid,
                            EmployeeId = cwt.Candidate.EmployeeId,
                            EmployeeFirstName = cwt.Candidate.FirstName,
                            EmployeeLastName = cwt.Candidate.LastName,
                            JobOrderId = cwt.JobOrderId,
                            JobTitle = cwt.JobOrder.JobTitle,
                            CompanyId = cwt.CompanyId,
                            CompanyLocationId = cwt.CompanyLocationId,
                            CompanyDepartmentId = cwt.JobOrder.CompanyDepartmentId,
                            CompanyContactId = cwt.CompanyContactId,
                            ShiftId = cwt.ShiftId,
                            JobStartDateTime = cwt.JobStartDateTime,
                            JobEndDateTime = cwt.JobEndDateTime,
                            ClockIn = cwt.ClockIn,
                            ClockOut = cwt.ClockOut,
                            NetWorkTimeInHours = cwt.NetWorkTimeInHours,
                            AdjustmentInMinutes = cwt.AdjustmentInMinutes,
                            ClockTimeInHours = cwt.ClockTimeInHours,
                            CandidateWorkTimeStatusId = cwt.CandidateWorkTimeStatusId,
                            UpdatedOnUtc = cwt.UpdatedOnUtc,
                            CreatedOnUtc = cwt.CreatedOnUtc,
                            AllowSuperVisorModifyWorkTime =cwt.JobOrder.AllowSuperVisorModifyWorkTime,
                            FranchiseId = cwt.FranchiseId,
                            Note = cwt.Note,
                            SignatureBy = cwt.SignatureBy,
                            SignatureByName = cwt.SignatureByName,
                            SignatureOnUtc = cwt.SignatureOnUtc
                        });

            return result;
        }


        public IQueryable<CandidateWorkTime> GetOpenCandidateWorkTimeByAccountForApproval(DateTime startDate, Account account = null, bool submittedOnly = false)
        {
            var endDate = startDate.AddDays(7);
            var result = GetAllCandidateWorkTimeByAccountAsQueryable(account, true)
                    .Where(x => x.NetWorkTimeInHours != 0)
                    .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched)
                    .Where(x => !submittedOnly || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval)
                    .Where(x => x.JobStartDateTime >= startDate && x.JobStartDateTime < endDate);
            return result.OrderBy(x => x.JobStartDateTime);
        }


        public IQueryable<CandidateWorkTime> GetOpenCandidateWorkTimeByAccountForApprovalByWeekStartDate(DateTime weekStartDate, Account account = null,
                                                                                                         bool submittedOnly = false)
        {
            var setting = _settingService.GetSettingByKey<int>("CandidateWorkTimeSettings.ApprovalWindowInDays");

            DateTime weekEndDate = weekStartDate.AddDays(7);

            var result = GetAllCandidateWorkTimeByAccountAsQueryable(account, true)
                    .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched)
                    .Where(x => !submittedOnly || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval)
                    .Where(x => x.JobStartDateTime >= weekStartDate && x.JobStartDateTime <= weekEndDate); // added range as we are only getting data of one week in this method.

            return result.OrderBy(x => x.JobStartDateTime);
        }


        public IQueryable<EmployeeWorkTimeApproval> GetEmployeeWorkTimeApprovalAsQueryable(DateTime startDate, DateTime endDate, Account account = null, bool showRejected = false)
        {
            endDate = endDate.AddDays(1);   // facilitate comparison
            var result = GetDailyTimeSheetsByAccountAsQueryable(account, showRejected)
                .Where(x => x.JobStartDateTime >= startDate && x.JobStartDateTime < endDate);
            return result;
        }


        public IQueryable<EmployeeWorkTimeApproval> GetEmployeeWorkTimeApprovalAsQueryable(DateTime weekStartDate, Account account = null, bool showRejected = false,
            int? candidateId = null, bool submittedOnly = false)
        {
            var weekEndDate = weekStartDate.AddDays(6);
            var result = GetEmployeeWorkTimeApprovalAsQueryable(weekStartDate, weekEndDate, account, showRejected)
                .Where(x => !candidateId.HasValue || x.CandidateId == candidateId.Value)
                .Where(x => !submittedOnly || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval);
            return result;
        }


        public IQueryable<EmployeeWorkTimeApproval> GetExpectedTimeSheets(Account account, DateTime weekStartDate, bool startedOnly = true)
        {
            var year = CommonHelper.GetYearAndWeekNumber(weekStartDate, out int week);
            var weekEndDate = weekStartDate.AddDays(6);
            var afterWeekEnd = weekEndDate.AddDays(1);

            var hours = GetAllCandidateWorkTimeByAccountAsQueryable(account, showRejected: true)
                .Where(x => x.JobStartDateTime >= weekStartDate && x.JobStartDateTime < afterWeekEnd);
            var created = hours.Select(x => new
            {
                CandidateId = x.CandidateId,
                JobOrderId = x.JobOrderId,
                StartDate = (DateTime)DbFunctions.TruncateTime(x.JobStartDateTime),
            });

            var placements = _candidateJobOrderService.GetAllCandidateJobOrdersByDateRangeAsQueryable(weekStartDate, weekEndDate).FilterByClientAccount(account);
            var dailyPlacments = _candidateJobOrderService.GetDailyPlacments(placements, weekStartDate, weekEndDate, workDayOnly: true);
            if (startedOnly)
                dailyPlacments = dailyPlacments.Where(x => DateTime.Now > 
                    (DateTime)DbFunctions.CreateDateTime(x.StartDate.Year, x.StartDate.Month, x.StartDate.Day, x.JobOrder.StartTime.Hour, x.JobOrder.StartTime.Minute, x.JobOrder.StartTime.Second));

            var expected = from dp in dailyPlacments
                           join c in created on new { x = dp.CandidateId, y = dp.JobOrderId, z = dp.StartDate } equals new { x = c.CandidateId, y = c.JobOrderId, z = c.StartDate } into joined
                           from j in joined.DefaultIfEmpty()
                           where j == null
                           select dp;

            var result = expected.Select(x => new EmployeeWorkTimeApproval()
            {
                Id = 0,
                Year = year,
                WeekOfYear = week,
                Payroll_BatchId = null,
                CandidateId = x.CandidateId,
                CandidateGuid = x.Candidate.CandidateGuid,
                EmployeeId = x.Candidate.EmployeeId,
                EmployeeFirstName = x.Candidate.FirstName,
                EmployeeLastName = x.Candidate.LastName,
                JobOrderId = x.JobOrderId,
                JobTitle = x.JobOrder.JobTitle,
                CompanyId = x.JobOrder.CompanyId,
                CompanyLocationId = x.JobOrder.CompanyLocationId,
                CompanyDepartmentId = x.JobOrder.CompanyDepartmentId,
                CompanyContactId = x.JobOrder.CompanyContactId,
                ShiftId = x.JobOrder.ShiftId,
                JobStartDateTime = (DateTime)DbFunctions.CreateDateTime(x.StartDate.Year, x.StartDate.Month, x.StartDate.Day, x.JobOrder.StartTime.Hour, x.JobOrder.StartTime.Minute, x.JobOrder.StartTime.Minute),
                JobEndDateTime = (DateTime)DbFunctions.CreateDateTime(x.EndDate.Year, x.EndDate.Month, x.EndDate.Day, x.JobOrder.EndTime.Hour, x.JobOrder.EndTime.Minute, x.JobOrder.EndTime.Minute),
                ClockIn = null,
                ClockOut = null,
                NetWorkTimeInHours = 0,
                AdjustmentInMinutes = 0,
                ClockTimeInHours = 0,
                CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval,
                UpdatedOnUtc = null,
                CreatedOnUtc = null,
                AllowSuperVisorModifyWorkTime = x.JobOrder.AllowSuperVisorModifyWorkTime,
                FranchiseId = x.JobOrder.FranchiseId,
                Note = null,
                SignatureBy = 0,
                SignatureByName = null,
                SignatureOnUtc = null
            });

            return result;
        }


        public IQueryable<CandidateWorkOverTime> GetCandidateOvertime(int[] candidateIds, int[] jobOrerId, DateTime startDate, DateTime endDate)
        {
            var _endDate = endDate.Date.AddHours(23d).AddMinutes(59d).AddSeconds(59d);
            var _startDate = startDate.Date;
            return _candidateWorkOverTimeRepository.Table.Where(x => candidateIds.Contains(x.CandidateId) &&
                x.EndDate <= _endDate
                && x.StartDate >= _startDate
                && x.Year != null && x.WeekOfYear != null);
        }


        public IQueryable<Alerts> GetCandidateUnacknowledgedAlerts(int[] candidateIds)
        {
            return _alertsRepository.Table.Where(x => candidateIds.Contains(x.CandidateId) && !x.Acknowledged);
        }


        public bool RejectWorkTimeEntry(int id, string reason, Account currentAccount)
        {
            bool result = false;
            try
            {
                var entry = _workTimeRepository.GetById(id);
                entry.Note = reason;

                ChangeCandidateWorkTimeStatus(entry, CandidateWorkTimeStatus.Rejected, currentAccount);
                result = true;

                try
                {
                    //activity log
                    _activityLogService.InsertActivityLog("RejectWorkTime", String.Format("Candidate ({0}) time sheet on {1} under Job order ({2}) is rejected.",
                                                            entry.CandidateId, entry.JobStartDateTime.ToShortDateString(), entry.JobOrderId
                                                        ));
                    _workflowMessageService.SendWorkTimeRejectionRecruiterNotification(entry, 1, currentAccount);
                }
                catch (Exception ex)
                {
                    _logger.Error(String.Format("RejectWorkTimeEntry(): Time sheet id {0} is rejected but other operations failed.", id), ex, currentAccount);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RejectWorkTimeEntry(): Failed to reject time sheet", ex, currentAccount);
            }

            return result;
        }


        public bool ApproveWorkTimeEntry(int id, Account currentAccount, out string errorMessage)
        {
            bool result = false;
            errorMessage = String.Empty;

            try
            {
                var entry = _workTimeRepository.GetById(id);
                ChangeCandidateWorkTimeStatus(entry, CandidateWorkTimeStatus.Approved, currentAccount);
                result = true; // because at this point the time sheet is actually approved. Even if the other parts fail

                try
                {
                    //activity log
                    _activityLogService.InsertActivityLog("ApproveWorkTime", String.Format("Candidate ({0}) time sheet on {1} under Job order ({2}) is approved.",
                                                                      entry.CandidateId, entry.JobStartDateTime.ToShortDateString(), entry.JobOrderId
                                                        ));

                    if (IsValidWorkTime(entry))
                    {
                        RemoveOtherMatchedWorkTimes(entry);
                        SetClockTimeStatusByWorkTime(entry);
                    }

                }
                catch (WfmException ex0)
                {
                    errorMessage = ex0.Message;
                }
                catch (Exception ex1)
                {
                    errorMessage = _localizationService.GetResource("Common.UnexpectedError");
                    _logger.Error(String.Format("ApproveWorkTimeEntry(): Time sheet id {0} is approved but other operations failed", id), ex1, currentAccount);
                }
            }
            catch (Exception ex)
            {
                errorMessage = _localizationService.GetResource("Common.UnexpectedError");
                _logger.Error("ApproveWorkTimeEntry(): Failed to approve time sheet", ex, currentAccount);
            }

            return result;
        }

        public bool SignWorkTimeEntry(int id, Account currentAccount, out string errorMessage)
        {
            bool result = false;
            errorMessage = String.Empty;

            try
            {
                var entry = _workTimeRepository.GetById(id);
                UpdateSignature(id, currentAccount);
                result = true;

                try
                {
                    _activityLogService.InsertActivityLog("SignWorkTime",
                        String.Format("Candidate ({0}) time sheet on {1} under Job order ({2}) is signed.",
                                entry.CandidateId, entry.JobStartDateTime.ToShortDateString(), entry.JobOrderId));
                }
                catch (WfmException ex0)
                {
                    errorMessage = ex0.Message;
                }
                catch (Exception ex1)
                {
                    errorMessage = _localizationService.GetResource("Common.UnexpectedError");
                    _logger.Error(String.Format("SignWorkTimeEntry(): Time sheet id {0} is signed but other operations failed", id), ex1, currentAccount);
                }
            }
            catch (Exception ex)
            {
                errorMessage = _localizationService.GetResource("Common.UnexpectedError");
                _logger.Error("SignWorkTimeEntry(): Failed to sign time sheet", ex, currentAccount);
            }

            return result;
        }

        private void UpdateSignature(int id, Account currentAccount)
        {
            string query = $"Update CandidateWorkTime " +
                $"Set [SignatureBy] = {currentAccount.Id}, [SignatureByName] = '{currentAccount.FirstName} {currentAccount.LastName}', [SignatureOnUtc] = GETUTCDATE() " +
                $"Where id = {id}";

            _dbContext.ExecuteSqlCommand(query, false, null);
        }

        #endregion


        #region Capture Candidate Work Time based on punch clock

        /// <summary>
        /// Captures the candidate work time.
        /// </summary>
        /// <param name="selectedStartDate">The selected start date.</param>
        /// <param name="selectedEndDate">The selected end date.</param>
        /// <returns></returns>
        public IList<string> CaptureCandidateWorkTime(DateTime startDate, DateTime endDate, Account account = null, bool includePlaced = false, bool includeNotPlaced = false)
        {
            IList<string> result = new List<string>();      // to store error messages

            if (startDate > endDate)
            {
                result.Add("Start date cannot be after end date");
                return result;
            }

            #region Clean up

            CleanupInvalidWorkTimes(startDate.Date, endDate.Date, includeNotPlaced);

            #endregion

            #region Process

            // get all active job orders between start and end date with punch clock time newer than last calculation
            DateTime startTimeUtc = DateTime.UtcNow;
            List<int> companyLocationIdNeedsUpdate = new List<int>();


            IQueryable<JobOrder> jobOrders = _jobOrderService.GetJobOrdersForWorkTimeCalculation(startDate.Date, endDate.Date);

            // limit the total job orders to be processed per batch to avoid overload the server
            const int pageSize = 25;
            int total = jobOrders.Count();
            int totalPages = total / pageSize;
            if (total % pageSize > 0) totalPages++;

            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
            {
                var jobOrderList = jobOrders.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                //_activityLogService.InsertActivityLog("CalculateCandidateWorkTime",
                //    string.Format("Job Orders being processed ({0}) at {1}",
                //    string.Join(", ", jobOrderList.Select(x => x.Id.ToString()).ToArray()),
                //    DateTime.UtcNow));

                foreach (var jobOrder in jobOrderList)
                {
                    // skip Closed job order
                    if (jobOrder != null && jobOrder.JobOrderStatusId == (int)JobOrderStatusEnum.Closed)
                        continue;

                    // validate job order start/end time, if the calculation is started by user
                    if (account != null)
                    {
                        if (jobOrder.StartTime == null || jobOrder.EndTime == null)
                        {
                            result.Add(string.Format("Job order {0} contains invalid start time or end time", jobOrder.Id));
                            continue;
                        }
                        if ((jobOrder.EndTime - jobOrder.StartTime).Duration().Hours < 1)
                        {
                            result.Add(string.Format("Job order {0} total duration is less than 1 hour", jobOrder.Id));
                            continue;
                        }
                    }

                    CalculateJobOrderCandidateWorkTime(jobOrder, startDate.Date, endDate.Date, account, false, includePlaced, includeNotPlaced);

                    if (!companyLocationIdNeedsUpdate.Contains(jobOrder.CompanyLocationId))
                        companyLocationIdNeedsUpdate.Add(jobOrder.CompanyLocationId);
                }
            }

            // update last calculation timestamp
            if (companyLocationIdNeedsUpdate.Count > 0)
                this.UpdateCompanyLocations(companyLocationIdNeedsUpdate, startTimeUtc);


            #endregion

            return result;
        }

        private void UpdateCompanyLocations(List<int> companyLocationIdNeedsUpdate, DateTime startTimeUtc)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("startTimeUtc", startTimeUtc) { SqlDbType = System.Data.SqlDbType.DateTime });

            var targetIds = String.Join(",", companyLocationIdNeedsUpdate);
            string query = String.Concat(
                            @"Update CompanyLocation 
                              Set LastWorkTimeCalculationDateTimeUtc = GETUTCDATE()
                              Where id in (", targetIds, ") and LastPunchClockFileUploadDateTimeUtc < @startTimeUtc ");

            _dbContext.ExecuteSqlCommand(query, false, null, parameters.ToArray());
        }

        public void CleanupInvalidWorkTimes(DateTime startDate, DateTime endDate, bool includeMatched, int jobOrderId = 0)
        {
            var canWorkTimes = GetWorkTimeByStartEndDateAsQueryable(startDate.Date, endDate.Date, includeMatched);
            if (jobOrderId > 0)
                canWorkTimes = canWorkTimes.Where(c => c.JobOrderId == jobOrderId);

            // skip
            canWorkTimes = canWorkTimes.Where(c => c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Approved &&
                                                   c.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided &&
                                                   (c.JobOrder != null && c.JobOrder.JobOrderStatusId != (int)JobOrderStatusEnum.Closed));

            // skip: valid work time (placed)
            canWorkTimes = canWorkTimes.Where(c => c.JobOrder.JobOrderStatusId != (int)JobOrderStatusEnum.Active ||
                                                   !c.JobOrder.CandidateJobOrders.Any(j => j.CandidateId == c.CandidateId &&
                                                                                      j.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                                                                      j.StartDate <= DbFunctions.TruncateTime(c.JobStartDateTime) &&
                                                                                      (!j.EndDate.HasValue || j.EndDate.Value >= DbFunctions.TruncateTime(c.JobStartDateTime))
                                                                                      ));

            canWorkTimes = canWorkTimes.Where(c => !c.IsLocked);
            var workTimeDbSet = _dbContext.Set<CandidateWorkTime>() as DbSet<CandidateWorkTime>;
            workTimeDbSet.RemoveRange(canWorkTimes);
            _dbContext.SaveChanges();
        }


        public void RemoveOtherMatchedWorkTimes(CandidateWorkTime cwt, bool includeSubmitted = true)
        {
            if (!cwt.ClockIn.HasValue && !cwt.ClockOut.HasValue)
                return;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("JobOrderId", cwt.JobOrderId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("CandidateId", cwt.CandidateId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("ClockIn", cwt.ClockIn) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("ClockOut", cwt.ClockOut) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("Matched", (int)CandidateWorkTimeStatus.Matched) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("Submitted", (int)CandidateWorkTimeStatus.PendingApproval) { SqlDbType = System.Data.SqlDbType.Int });

            if (!cwt.ClockIn.HasValue)
                parameters.Find(x => x.ParameterName == "ClockIn").Value = DBNull.Value;
            if (!cwt.ClockOut.HasValue)
                parameters.Find(x => x.ParameterName == "ClockOut").Value = DBNull.Value;

            var query = new StringBuilder();
            query.AppendLine(@"
                                declare @targetIds table (cwtId int)
                                insert @targetIds
	                                select Id from CandidateWorkTime
	                                where JobOrderId != @JobOrderId
	                                and CandidateId = @CandidateId
	                                and ((@ClockIn is not null and (ClockIn = @ClockIn or ClockOut = @ClockIn)) or 
                                         (@ClockOut is not null and (ClockIn = @ClockOut or ClockOut = @ClockOut)))
                            ");

            if (includeSubmitted)
                query.AppendLine(@" and (CandidateWorkTimeStatusId = @Matched or CandidateWorkTimeStatusId = @Submitted) ");
            else
                query.AppendLine(@" and CandidateWorkTimeStatusId = @Matched ");

            query.AppendLine(@"
                                delete from CandidateWorkTime
                                where Id in (select cwtId from @targetIds) and isLocked=0
                            ");

            _dbContext.ExecuteSqlCommand(query.ToString(), false, null, parameters.ToArray());
        }

        public void RemoveMatchedWorktimeForAfterFactTimeoffBooking(int candidateId, DateTime beginDate, DateTime endDate)
        {
            var matchedStatusId = (int)(CandidateWorkTimeStatus.Matched);
            var entries = _workTimeRepository.Table
                .Where(x => x.CandidateId == candidateId)
                .Where(x => x.JobStartDateTime >= beginDate)
                .Where(x => x.JobEndDateTime <= endDate)
                .Where(x => x.CandidateWorkTimeStatusId == matchedStatusId)
                .ToArray();
            _workTimeRepository.Delete(entries);
        }

        //private IQueryable<CandidateWorkTime> GetOtherMatchedWorkTimes(CandidateWorkTime cwt, bool includeSubmitted = false)
        //{
        //    // offset rounding
        //    //var minIn = cwt.ClockIn.Value.AddMinutes(-1);
        //    //var maxIn = cwt.ClockIn.Value.AddMinutes(+1);
        //    //var minOut = cwt.ClockOut.Value.AddMinutes(-1);
        //    //var maxOut = cwt.ClockOut.Value.AddMinutes(+1);
        //    var query = _workTimeRepository.Table;

        //    if (includeSubmitted)
        //        query = query.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Submitted);
        //    else
        //        query = query.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched);

        //    query = query.Where(x => x.JobOrderId != cwt.JobOrderId && x.CandidateId == cwt.CandidateId &&
        //                             //((x.ClockIn >= minIn && x.ClockIn <= maxIn) || (x.ClockOut >= minOut && x.ClockOut <= maxOut) || 
        //                             // (x.ClockIn >= minOut && x.ClockIn <= maxOut) || (x.ClockOut >= minIn && x.ClockOut <= maxIn)));
        //                             (x.ClockIn == cwt.ClockIn || x.ClockOut == cwt.ClockOut || x.ClockIn == cwt.ClockOut || x.ClockOut == x.ClockIn));

        //    return query;
        //}

        public void SetClockTimeStatusByWorkTime(CandidateWorkTime cwt, int statusId = (int)CandidateClockTimeStatus.Processed, bool isForRescheduling = false)
        {
            var clockTimes = _clockTimeService.GetAllClockTimesByCandidateIdAndLocationIdAndDateTimeRange(cwt.CandidateId, cwt.CompanyLocationId, cwt.ClockIn, cwt.ClockOut);

            _clockTimeService.UpdateClockTimeStatus(clockTimes, statusId, cwt.UpdatedBy, isForRescheduling);
        }

        public void CalculateJobOrderCandidateWorkTime(JobOrder jobOrder, DateTime startDate, DateTime endDate, Account account = null, bool cleanupWorkTimes = true, bool includePlaced = false, bool includeNotPlaced = false)
        {
            if (cleanupWorkTimes)
                CleanupInvalidWorkTimes(startDate, endDate, includeNotPlaced, jobOrder.Id);

            // base CandidateWorkTime, with common fields determined by job order and date
            var cwt = PrepareCandidateWorkTimeByJobOrderAndDate(account, jobOrder, startDate);

            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                // for placed candidates
                if (includePlaced)
                    this.CalculateWorkTimeForPlacedCandidates(cwt);

                // for candidates not placed, but with punches in shift range
                if (includeNotPlaced)
                    this.CalculateWorkTimeForNotPlacedCandidates(cwt, jobOrder);

                cwt.JobStartDateTime = cwt.JobStartDateTime.AddDays(1);
                cwt.JobEndDateTime = cwt.JobEndDateTime.AddDays(1);
                cwt.Year = CommonHelper.GetYearAndWeekNumber(cwt.JobStartDateTime, out int weekOfYear);
                cwt.WeekOfYear = weekOfYear;

                currentDate = currentDate.AddDays(1);
            }
        }
        
        
        public CandidateWorkTime PrepareCandidateWorkTimeByJobOrderAndDate(Account account, JobOrder jobOrder, DateTime nDate)
        {
            #region Define Scanning time window

            var jobStartDateTime = nDate.Date + jobOrder.StartTime.TimeOfDay;
            var jobEndDateTime = nDate.AddDays(jobOrder.StartTime.TimeOfDay > jobOrder.EndTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay;

            #endregion


            #region Retrieve policy information

            var schedulePolicy = _schedulePolicyService.GetSchedulePolicyById(jobOrder.SchedulePolicyId);
            if (schedulePolicy == null)
                schedulePolicy = new SchedulePolicy { RoundingPolicyId = 0, MealPolicyId = 0, BreakPolicyId = 0, IsStrictSchedule = true, OvertimeGracePeriodInMinutes = 0 };

            decimal overtimeGracePeriodInMinutes = schedulePolicy.OvertimeGracePeriodInMinutes;
            decimal mealTimeInMinutes = 0m;
            var minWorkHoursForMealBreak = _GetMinWorkHoursForMealBreak(schedulePolicy, out mealTimeInMinutes);
            decimal breakTimeInMinutes = schedulePolicy.BreakPolicyId == 0 ? 0m : _breakPolicyService.GetBreakPolicyById(schedulePolicy.BreakPolicyId).BreakTimeInMinutes;
            decimal roundingIntervalInMinutes = 0m;
            decimal roundingGracePeriodInMinutes = 0m;
            if (schedulePolicy.RoundingPolicyId > 0)
            {
                var roundingPolicy = _roundingPolicyService.GetRoundingPolicyById(schedulePolicy.RoundingPolicyId);
                roundingIntervalInMinutes = roundingPolicy.IntervalInMinutes;
                roundingGracePeriodInMinutes = roundingPolicy.GracePeriodInMinutes;
            }

            #endregion

            return new CandidateWorkTime()
            {
                JobOrderId = jobOrder.Id,
                ShiftId = jobOrder.ShiftId,

                CompanyId = jobOrder.CompanyId,
                CompanyLocationId = jobOrder.CompanyLocationId,
                CompanyDepartmentId = jobOrder.CompanyDepartmentId,
                CompanyContactId = jobOrder.CompanyContactId,

                JobStartDateTime = jobStartDateTime,
                JobEndDateTime = jobEndDateTime,
                Year = CommonHelper.GetYearAndWeekNumber(jobStartDateTime, out int weekOfYear),
                WeekOfYear = weekOfYear,
                JobOrderDurationInMinutes = (decimal)(jobEndDateTime - jobStartDateTime).TotalMinutes,
                JobOrderDurationInHours = (decimal)(jobEndDateTime - jobStartDateTime).TotalHours,

                IsStrictSchedule = schedulePolicy.IsStrictSchedule,
                OvertimeGracePeriodInMinutes = schedulePolicy.OvertimeGracePeriodInMinutes,
                MinWorkHoursForMealBreak = minWorkHoursForMealBreak,
                MealTimeInMinutes = mealTimeInMinutes,
                BreakTimeInMinutes = breakTimeInMinutes,
                GracePeriodInMinutes = roundingGracePeriodInMinutes,
                RoundingIntervalInMinutes = roundingIntervalInMinutes,

                EnteredBy = account == null ? 0 : account.Id,
                FranchiseId = jobOrder.FranchiseId,

                Payroll_BatchId = null,
                InvoiceDate = null
            };
        }

        public decimal GetMinWorkHoursForMealBreak(int jobOrderId)
        {
            var minworkHours = 0m;
            var mealBreak = 0m;
            var schedulePolicy = _schedulePolicyService.GetSchedulePolicieByJobOrderId(jobOrderId);
            if (schedulePolicy != null)
                minworkHours = _GetMinWorkHoursForMealBreak(schedulePolicy, out mealBreak);

            return minworkHours;
        }

        private decimal _GetMinWorkHoursForMealBreak(SchedulePolicy schedulePolicy, out decimal mealTimeInMinutes)
        {
            if (schedulePolicy == null)
                schedulePolicy = new SchedulePolicy { RoundingPolicyId = 0, MealPolicyId = 0, BreakPolicyId = 0, IsStrictSchedule = true, OvertimeGracePeriodInMinutes = 0 };

            var mealPolicy = _mealPolicyService.GetMealPolicyById(schedulePolicy.MealPolicyId);
            mealTimeInMinutes = mealPolicy != null ? mealPolicy.MealTimeInMinutes : 0m;
            var minWorkHoursForMealBreak = mealPolicy != null && mealPolicy.MinWorkHours > 0 ? mealPolicy.MinWorkHours : _candidateWorkTimeSettings.MealBreakThreshold;

            return minWorkHoursForMealBreak;
        }

        private void CalculateWorkTimeForPlacedCandidates(CandidateWorkTime cwt)
        {
            var uniqueCandidateIdList = this.GetListOfPlacedCandidateForProcessing(cwt.JobOrderId, cwt.JobStartDateTime.Date);
            if (uniqueCandidateIdList.Count > 0)
            {
                cwt.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval;
                CalculateWorkTimeForCandidates(uniqueCandidateIdList, cwt);
            }
        }

        private void CalculateWorkTimeForNotPlacedCandidates(CandidateWorkTime cwt, JobOrder jobOrder)
        {
            var refDate = cwt.JobStartDateTime.Date;

            var idList = this.GetListOfCandidateWhoAreNotPlaced(jobOrder, refDate, true);
            if (idList.Count > 0)
            {
                cwt.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.Matched;
                CalculateWorkTimeForCandidates(idList, cwt);
            }
        }

        private List<int> GetListOfPlacedCandidateForProcessing(int jobOrderId, DateTime refdate)
        {
            List<int> result = new List<int>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("jobOrderId", jobOrderId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("refDate", refdate) { SqlDbType = System.Data.SqlDbType.DateTime });


            const string query = @"Select distinct cjo.CandidateId
                                   from CandidateJobOrder cjo
                                      left outer join CandidateWorkTime cwt on cjo.CandidateId = cwt.CandidateId and cjo.JobOrderId = cwt.JobOrderId 
                                                                               and cwt.IsLocked = 1 and DATEDIFF(day, cwt.JobStartDateTime, @refDate) = 0 
                                      inner join CandidateClockTime ct on cjo.CandidateId = ct.CandidateId and DATEDIFF(day, ct.ClockInOut, @refDate) = 0
                                   Where cjo.JobOrderId = @jobOrderId
                                         and cjo.CandidateJobOrderStatusId = 12 -- placed
	                                     and cjo.StartDate <= @refDate
	                                     and (cjo.EndDate is null or cjo.EndDate>= @refDate)
	                                     and cwt.Id is Null
                                         and ct.IsDeleted = 0
                                  ";

            result = _dbContext.SqlQuery<int>(query.ToString(), parameters.ToArray()).ToList();
            return result;
        }

        private List<int> GetListOfCandidateWhoAreNotPlaced(JobOrder jobOrder, DateTime refDate, bool locationBound = true)
        {
            List<int> result = new List<int>();

            if (jobOrder == null)
                return result;

            var startTime = refDate.Date + jobOrder.StartTime.TimeOfDay;
            var endTime = refDate.AddDays(jobOrder.StartTime.TimeOfDay > jobOrder.EndTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay;
            var minStartTime = startTime.AddMinutes(-_candidateWorkTimeSettings.MatchBeforeStartTimeInMinutes);
            var maxStartTime = startTime.AddMinutes(_candidateWorkTimeSettings.MatchAfterStartTimeInMinutes);
            var minEndTime = endTime.AddMinutes(-_candidateWorkTimeSettings.MatchBeforeEndTimeInMinutes);
            var maxEndTime = endTime.AddMinutes(_candidateWorkTimeSettings.MatchAfterEndTimeInMinutes);

            var jobDuration = (endTime - startTime).TotalMinutes;
            var validThreshold = (decimal)jobDuration * _candidateWorkTimeSettings.ValidWorkTimeRatio;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CompanyLocationId", jobOrder.CompanyLocationId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("CompanyId", jobOrder.CompanyId) { SqlDbType = System.Data.SqlDbType.Int });
            parameters.Add(new SqlParameter("minStartTime", minStartTime) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("maxStartTime", maxStartTime) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("minEndTime", minEndTime) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("maxEndTime", maxEndTime) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("refDate", refDate) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("threshold", validThreshold) );

            var query = new StringBuilder();
            query.AppendLine(@"  Select Distinct cct.CandidateID 
                                 From CandidateClockTime cct 
                                      left outer join candidateWorkTime cwt on cct.CandidateId = cwt.CandidateId
	                                            and DATEDIFF(day, cwt.JobStartDateTime, @refDate) = 0
			                                    and cct.CompanyLocationId = cwt.CompanyLocationId
                                                and Cwt.CandidateWorkTimeStatusId <> 10 -- 10 means Matched
                                                and Cwt.GrossWorkTimeInMinutes > @threshold
                                 Where ");

            if (locationBound)
                query.AppendLine(@" cct.CompanyLocationId = @CompanyLocationId ");
            else
                query.AppendLine(@" cct.CompanyId = @CompanyId ");

            query.AppendLine(@" and cct.IsDeleted = 0");

            query.AppendLine(@" and cct.CandidateId is not Null
                                and CandidateClockTimeStatusId = 0 -- NoStatus
                                and ((ClockInOut > @minStartTime and ClockInOut < @maxStartTime) OR 
                                     (ClockInOut > @minEndTime and ClockInOut < @maxEndTime)
                                    )
	                            and cwt.id is null
                              ");


            result = _dbContext.SqlQuery<int>(query.ToString(), parameters.ToArray()).ToList();

            return result;
        }


        public void CalculateWorkTimeForCandidates(IList<int> candidateIdList, CandidateWorkTime cwt)
        {
            #region Define Scanning time window

            var scanStartDateTime = cwt.JobStartDateTime.AddMinutes(-_candidateWorkTimeSettings.StartScanWindowSpanInMinutes);
            var scanEndDateTime = cwt.JobEndDateTime.AddMinutes(_candidateWorkTimeSettings.EndScanWindowSpanInMinutes);

            #endregion

            bool _noStausRecordsOnly = (cwt.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched);

            // get in& out times for candidates
            var times = this._getCandidateInOutTimes(cwt.CompanyId, candidateIdList, scanStartDateTime, scanEndDateTime, _noStausRecordsOnly);
            foreach (var inOutTimes in times)
                this.CalculateWorkTimeForOneCandidate(cwt, inOutTimes, scanStartDateTime, scanEndDateTime);
        }

         
        public void CalculateWorkTimeForOneCandidate(CandidateWorkTime cwt, InOutTimes inOutTimes,
                                                     DateTime scanStartDateTime, DateTime scanEndDateTime)
        {
            bool _noStausRecordsOnly = (cwt.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched);

            if (inOutTimes != null)
            {
                // any existing record for the candidate/joborder/date
                var candidateWorkTime = GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(inOutTimes.CandidateId, cwt.JobOrderId, cwt.JobStartDateTime.Date,
                                            includeMatched: cwt.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched);

                // skip Approved or Rejected or Voided work time or pending with adjustment, for update
                if (candidateWorkTime != null && (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved ||
                                                  candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Rejected ||
                                                  candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Voided   ||
                                                  candidateWorkTime.AdjustmentInMinutes != 0))
                    return;

                #region DST adjustment

                // Check DST
                bool isDST1 = TimeZoneInfo.Local.IsDaylightSavingTime(inOutTimes.ClockInTime);
                bool isDST2 = TimeZoneInfo.Local.IsDaylightSavingTime(inOutTimes.ClockOutTime);

                bool isExitingDST = (isDST1 == true && isDST2 == false);
                bool isEnteringDST = (isDST1 == false && isDST2 == true);

                if (isExitingDST)
                    inOutTimes.ClockOutTime = inOutTimes.ClockOutTime.AddHours(1);
                if (isEnteringDST)
                    inOutTimes.ClockOutTime = inOutTimes.ClockOutTime.AddHours(-1);

                #endregion

                #region Calculate and Save work time

                UpdateWorkTimeFromBaseWorkTime(cwt, ref candidateWorkTime);

                candidateWorkTime.ClockIn = inOutTimes.ClockInTime;
                candidateWorkTime.ClockOut = inOutTimes.ClockOutTime;
                candidateWorkTime.ClockDeviceUid = inOutTimes.ClockDeviceUid;
                candidateWorkTime.SmartCardUid = inOutTimes.SmartCardUid;
                candidateWorkTime.Source = inOutTimes.Source;

                candidateWorkTime.CandidateId = inOutTimes.CandidateId;

                CalculateAndSaveWorkTime(candidateWorkTime);

                #endregion

                #region Cleanup Matched WorkTime, and Update Clock Time Status

                if (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval && IsValidWorkTime(candidateWorkTime))
                {
                    // cleanup other matched work times
                    RemoveOtherMatchedWorkTimes(candidateWorkTime);

                    // update status of all clock times within scope
                    this._updateCandidateClockTimes(cwt.CompanyId, inOutTimes.CandidateId, inOutTimes.ClockInTime, inOutTimes.ClockOutTime, 
                        _noStausRecordsOnly, (int)CandidateClockTimeStatus.Processed, candidateWorkTime.UpdatedBy, false);
                }

                #endregion
            }
        }


        /// <summary>
        /// Returns punch in & out times for seleted candidates within a specific date and time range
        /// for a specific company, regardless of location, device, or smart card
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="candidateIds"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<InOutTimes> _getCandidateInOutTimes(int companyId, IList<int> candidateIds, DateTime startTime, DateTime endTime, bool noStausRecordsOnly)
        {
            var allClockTimes = _clockTimeService.GetAllCandidateClockTimesAsQueryable().Where(x => !x.IsRescheduleClockTime)
                .Where(x => x.CandidateClockTimeStatusId != (int)CandidateClockTimeStatus.NotOnboarded)
                .Where(x => x.CompanyId == companyId && x.CandidateId.HasValue && candidateIds.Contains(x.CandidateId.Value))
                .Where(x => x.ClockInOut >= startTime && x.ClockInOut <= endTime);

            if (noStausRecordsOnly)
                allClockTimes = allClockTimes.Where(x => x.CandidateClockTimeStatusId == (int)CandidateClockTimeStatus.NoStatus);

            // only those from active cards, or alt.ID
            var cards = _smartCardService.GetAllSmartCardsAsQueryable(showInactive: false);
            var clockTimes = (from ct in allClockTimes
                             from c in cards.Where(c => ct.SmartCardUid == c.SmartCardUid).DefaultIfEmpty()
                             where c != null || ct.SmartCardUid.StartsWith("ID")
                             orderby ct.ClockInOut
                             select ct).ToList();

            return clockTimes.GroupBy(x => x.CandidateId).Select(g => new InOutTimes()
            {
                CandidateId = g.Key.Value,
                SmartCardUid = g.First().SmartCardUid,
                ClockInTime = g.First().ClockInOut,
                ClockOutTime = g.Last().ClockInOut,
                ClockDeviceUid = g.First().ClockDeviceUid,
                Source = g.First().Source
            }).ToList();
        }


        private void _updateCandidateClockTimes(int companyId, int candidateId, DateTime startTime, DateTime endTime, bool noStausRecordsOnly,
                                                int statusId, int updatedBy, bool isForRescheduling)
        {
            SqlParameter[] paras = new SqlParameter[7];
            paras[0] = new SqlParameter("candidateId", candidateId);
            paras[1] = new SqlParameter("companyId", companyId);
            paras[2] = new SqlParameter("startTime", startTime);
            paras[3] = new SqlParameter("endTime", endTime);
            paras[4] = new SqlParameter("statusId", statusId);
            paras[5] = new SqlParameter("updatedBy", updatedBy);
            paras[6] = new SqlParameter("isForRescheduling", isForRescheduling);

            string _FilterOnStatus = noStausRecordsOnly ? " and CandidateClockTimeStatusId == 0 -- NoStatus" : "";

            // regardless of location, device, or smart card
            var result = _dbContext.ExecuteSqlCommand(String.Concat(@"
                update CandidateClockTime 
                    set 
                        CandidateClockTimeStatusId = @statusId,
                        UpdatedBy = @updatedBy,
                        UpdatedOnUtc = GETUTCDATE(),
                        IsRescheduleClockTime = @isForRescheduling
                    where CompanyId = @companyId and CandidateId = @candidateId
	                    and IsDeleted = 0 and IsRescheduleClockTime = 0
	                    and ClockInOut >= @startTime and ClockInOut <= @endTime
                        and CandidateClockTimeStatusId != 99 -- NotOnboarded",
                        _FilterOnStatus),
                false, 0, paras);
        }


        public void CalculateAndSaveWorkTime(CandidateWorkTime candidateWorkTime, bool setLocked = false)
        {
            if (candidateWorkTime == null || candidateWorkTime.IsLocked)
                return;

            // skip Approved or Rejected or Voided work time, for update
            if (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved || 
                candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Rejected || 
                candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Voided)
                return;

            #region Validate Clock In/Out
            // missing in or out punch?
            bool missingInOrOut = false;

            var clockInTime = candidateWorkTime.ClockIn.HasValue ? candidateWorkTime.ClockIn.Value : DateTime.MinValue;
            var clockOutTime = candidateWorkTime.ClockOut.HasValue ? candidateWorkTime.ClockOut.Value : DateTime.MaxValue;
            if (!candidateWorkTime.ClockIn.HasValue || !candidateWorkTime.ClockOut.HasValue)
                missingInOrOut = true;

            if ((clockOutTime - clockInTime).TotalMinutes < 3)      // clockInTime nearly equals to clockOutTime
            {
                missingInOrOut = true;

                // if close to shift start
                if (Math.Abs((clockInTime - candidateWorkTime.JobStartDateTime).TotalMinutes) < Math.Abs((clockOutTime - candidateWorkTime.JobEndDateTime).TotalMinutes))
                    clockOutTime = DateTime.MaxValue;
                // if close to shift end
                else
                    clockInTime = DateTime.MinValue;
            }
            // both out of shift range
            if ((clockInTime < candidateWorkTime.JobStartDateTime && clockOutTime < candidateWorkTime.JobStartDateTime) ||
                (clockInTime > candidateWorkTime.JobEndDateTime && clockOutTime > candidateWorkTime.JobEndDateTime))
                missingInOrOut = true;

            // for guessed job order 
            if (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched)
                // skip calculation if any missing punches 
                if (missingInOrOut) return;
                // skip calculation if start time not match
                else if ((candidateWorkTime.JobStartDateTime - clockInTime).TotalMinutes > _candidateWorkTimeSettings.MatchBeforeStartTimeInMinutes ||
                    (clockInTime - candidateWorkTime.JobStartDateTime).TotalMinutes > _candidateWorkTimeSettings.MatchAfterStartTimeInMinutes)
                    return;

            #endregion

            #region Calculate work time

            decimal clockTimeInMinutes = 0;
            decimal grossWorkTimeInMinutes = 0;
            decimal lateInTimeInMinutes = 0;
            decimal earlyOutTimeInMinutes = 0;
            decimal totalAbsenceTimeInMinutes = 0;
            decimal lateOutTimeInMinutes = 0;

            // Round DOWN clock In seconds; disabled to easy job order matching
            //if (clockInTime > DateTime.MinValue)
            //    clockInTime = clockInTime.AddSeconds(-clockInTime.Second % 60);
            // Round UP clock Out seconds; disabled to easy job order matching
            //if (clockOutTime < DateTime.MaxValue)
            //    clockOutTime = clockOutTime.AddSeconds((60 - clockOutTime.Second) % 60);

            if (!missingInOrOut)
            {
                // Calculate punch time
                clockTimeInMinutes = Math.Round((decimal)(clockOutTime - clockInTime).TotalMinutes);

                if (candidateWorkTime.IsStrictSchedule)    // >>>*** Use Job Order time as work time ***<<<
                {
                    grossWorkTimeInMinutes = (decimal)(candidateWorkTime.JobEndDateTime - candidateWorkTime.JobStartDateTime).TotalMinutes;

                    // 0 for early or on-time in
                    lateInTimeInMinutes = (decimal)CalculateLateInTime(clockInTime, candidateWorkTime.JobStartDateTime, candidateWorkTime.GracePeriodInMinutes);

                    // negative for late out
                    earlyOutTimeInMinutes = (decimal)CalculateEarlyOutTime(clockOutTime, candidateWorkTime.JobEndDateTime, candidateWorkTime.GracePeriodInMinutes);

                    // check if it is overtime
                    if (earlyOutTimeInMinutes < 0)
                    {
                        lateOutTimeInMinutes = 0 - earlyOutTimeInMinutes;
                        earlyOutTimeInMinutes = 0;

                        if (candidateWorkTime.OvertimeGracePeriodInMinutes > 0)
                            lateOutTimeInMinutes = lateOutTimeInMinutes < candidateWorkTime.OvertimeGracePeriodInMinutes ? 0 : lateOutTimeInMinutes;

                        grossWorkTimeInMinutes += RoundLateOutTime(lateOutTimeInMinutes, candidateWorkTime.RoundingIntervalInMinutes);
                    }

                    // calculate absence time
                    totalAbsenceTimeInMinutes = RoundAbsenceTime(lateInTimeInMinutes + earlyOutTimeInMinutes, candidateWorkTime.RoundingIntervalInMinutes);

                    grossWorkTimeInMinutes -= totalAbsenceTimeInMinutes;
                    if (grossWorkTimeInMinutes < 0) grossWorkTimeInMinutes = 0m;
                }

                else    // >>>*** Use punch time as work time ***<<<
                    grossWorkTimeInMinutes = clockTimeInMinutes;
            }

            // for guessed job order, skip tiny or smaller work time 
            if (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched &&
                (grossWorkTimeInMinutes < 15 || DoesValidWorkTimeExist(candidateWorkTime.CandidateId, candidateWorkTime.JobStartDateTime.Date, clockInTime, clockOutTime, grossWorkTimeInMinutes)))
                //grossWorkTimeInMinutes < 15)
                return;

            candidateWorkTime.AdjustmentInMinutes = candidateWorkTime != null ? candidateWorkTime.AdjustmentInMinutes : 0m;

            // *** Net work time ***
            decimal netWorkTimeInMinutes = CalculateNetWorkTime(grossWorkTimeInMinutes, candidateWorkTime.MinWorkHoursForMealBreak, candidateWorkTime.MealTimeInMinutes, candidateWorkTime.BreakTimeInMinutes, candidateWorkTime.AdjustmentInMinutes);

            #endregion

            #region Create / Update Candidate Work Time record

            candidateWorkTime.ClockIn = clockInTime == DateTime.MinValue ? null : (DateTime?)clockInTime;
            candidateWorkTime.ClockOut = clockOutTime == DateTime.MaxValue ? null : (DateTime?)clockOutTime;

            candidateWorkTime.ClockTimeInMinutes = clockTimeInMinutes;
            candidateWorkTime.ClockTimeInHours = CommonHelper.RoundUp((decimal)TimeSpan.FromMinutes((double)clockTimeInMinutes).TotalHours, WORKTIME_DECIMAL_PLACES);

            candidateWorkTime.LateOutTimeInMinutes = lateOutTimeInMinutes;
            candidateWorkTime.LateOutTimeInHours = CommonHelper.RoundUp((decimal)TimeSpan.FromMinutes((double)lateOutTimeInMinutes).TotalHours, WORKTIME_DECIMAL_PLACES);

            candidateWorkTime.LateInTimeInMinutes = lateInTimeInMinutes;
            candidateWorkTime.EarlyOutTimeInMinutes = earlyOutTimeInMinutes;
            candidateWorkTime.TotalAbsenceTimeInMinutes = totalAbsenceTimeInMinutes;

            candidateWorkTime.GrossWorkTimeInMinutes = grossWorkTimeInMinutes;
            candidateWorkTime.GrossWorkTimeInHours = CommonHelper.RoundUp((decimal)TimeSpan.FromMinutes((double)grossWorkTimeInMinutes).TotalHours, WORKTIME_DECIMAL_PLACES);

            candidateWorkTime.NetWorkTimeInMinutes = (decimal)TimeSpan.FromMinutes((double)netWorkTimeInMinutes).TotalMinutes;
            candidateWorkTime.NetWorkTimeInHours = CommonHelper.RoundUp((decimal)TimeSpan.FromMinutes((double)netWorkTimeInMinutes).TotalHours, WORKTIME_DECIMAL_PLACES);

            candidateWorkTime.UpdatedBy = candidateWorkTime.EnteredBy;
            candidateWorkTime.UpdatedOnUtc = System.DateTime.UtcNow;

            // only submit valid work time
            //if (!IsValidWorkTime(candidateWorkTime))
            //    candidateWorkTime.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.Matched;
            candidateWorkTime.IsLocked = setLocked;
            if (candidateWorkTime.Id == 0)
            {
                candidateWorkTime.CreatedOnUtc = candidateWorkTime.UpdatedOnUtc;
                InsertCandidateWorkTime(candidateWorkTime);
            }
            else
                UpdateCandidateWorkTime(candidateWorkTime);

            #endregion
        }


        public bool IsValidWorkTime(CandidateWorkTime cwt)
        {
            var result = String.IsNullOrEmpty(cwt.SmartCardUid) ||
                //cwt.Source == WorkTimeSource.Manual ||
                         (cwt.NetWorkTimeInMinutes > 0 && cwt.NetWorkTimeInMinutes > cwt.ClockTimeInMinutes * _candidateWorkTimeSettings.ValidWorkTimeRatio);

            return result;
        }


        private void UpdateWorkTimeFromBaseWorkTime(CandidateWorkTime cwt, ref CandidateWorkTime candidateWorkTime)
        {
            Mapper.CreateMap<CandidateWorkTime, CandidateWorkTime>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    //.ForMember(dest => dest.MinWorkHoursForMealBreak, opt => opt.Ignore())
                    .ForMember(dest => dest.Note, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, opt => opt.Ignore())
                    .ForMember(dest => dest.Candidate, opt => opt.Ignore())
                    .ForMember(dest => dest.JobOrder, opt => opt.Ignore())
                    .ForMember(dest => dest.CandidateWorkTimeChanges, opt => opt.Ignore());
            candidateWorkTime = Mapper.Map<CandidateWorkTime, CandidateWorkTime>(cwt, candidateWorkTime);
        }


        //private IQueryable<int> GetIdOfCandidatesPlacedByDate(DateTime refDate)
        //{
        //    return _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable(refDate, (int)CandidateJobOrderStatusEnum.Placed)
        //           .Select(x => x.CandidateId).Distinct();
        //}


        //private IQueryable<int> GetIdOfCandidatesInPipelineByDate(DateTime refDate)
        //{
        //    return _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable(refDate, 0)
        //           .Select(x => x.CandidateId).Distinct();
        //}


        //private IQueryable<int> GetIdOfCandidatesWithValidWorkTimeByDate(DateTime refDate, decimal ratio)
        //{
        //    return GetWorkTimeByStartEndDateAsQueryable(refDate, refDate)
        //           .Where(x => (x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Submitted || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved) &&
        //                        x.ClockTimeInMinutes > 0 && x.GrossWorkTimeInMinutes >= x.ClockTimeInMinutes * ratio)
        //           .Select(x => x.CandidateId).Distinct();
        //}



        private bool DoesValidWorkTimeExist(int candidateId, DateTime refDate, DateTime clockIn, DateTime clockOut, decimal matchedGross)
        {
            var result = GetWorkTimeByCandidateIdAsQueryable(candidateId)
                         .Where(x => x.ClockIn == clockIn && x.ClockOut == clockOut &&
                                     x.GrossWorkTimeInMinutes >= matchedGross)
                         .Any();

            return result;
        }


        #endregion

        #region Helper methods

        //private bool IsClockInTime(DateTime clockDT, DateTime jobStartDT, DateTime jobEndDT)
        //{
        //    bool isClockIn = false;

        //    TimeSpan span1 = clockDT - jobStartDT;
        //    TimeSpan span2 = clockDT - jobEndDT;

        //    int result = TimeSpan.Compare(span1.Duration(), span2.Duration());
        //    if (result <= 0)
        //        isClockIn = true;
        //    else
        //        isClockIn = false;

        //    return isClockIn;
        //}


        //private bool IsClockOutTime(DateTime clockDT, DateTime jobStartDT, DateTime jobEndDT)
        //{
        //    bool isClockOut = false;

        //    TimeSpan span1 = clockDT - jobStartDT;
        //    TimeSpan span2 = clockDT - jobEndDT;

        //    int result = TimeSpan.Compare(span1.Duration(), span2.Duration());
        //    if (result <= 0)
        //        isClockOut = false;
        //    else
        //        isClockOut = true;

        //    return isClockOut;
        //}


        private double CalculateLateInTime(DateTime clockInDT, DateTime jobStartDT, decimal gracePeriodInMinutes = 0)
        {
            double totalMinutes = 0;

            TimeSpan span = clockInDT - jobStartDT;
            if (span.TotalMinutes > (double)gracePeriodInMinutes) // Apply grace period
                totalMinutes = span.TotalMinutes;

            return Math.Round(totalMinutes);
        }


        private double CalculateEarlyOutTime(DateTime clockOutDT, DateTime jobEndDT, decimal gracePeriodInMinutes = 0)
        {
            double totalMinutes = (jobEndDT - clockOutDT).TotalMinutes;

            if (totalMinutes > 0 && totalMinutes < (double)gracePeriodInMinutes) // Apply grace period
                totalMinutes = 0;

            return Math.Round(totalMinutes);
        }


        private decimal RoundLateOutTime(decimal originalLateOutTime, decimal roundingInterval)
        {
            if (roundingInterval <= 0) return originalLateOutTime;

            decimal result = originalLateOutTime;

            decimal reminder = originalLateOutTime % roundingInterval;
            // Rounding late out time (DOWN)
            result -= reminder;

            return result;
        }


        private decimal RoundAbsenceTime(decimal originalAbsenceTime, decimal roundingInterval)
        {
            if (roundingInterval <= 0) return originalAbsenceTime;

            decimal result = originalAbsenceTime;

            decimal reminder = originalAbsenceTime % roundingInterval;
            // Rounding absence time (UP)
            if (reminder >= 1)
                result = originalAbsenceTime - reminder + roundingInterval;

            return result;
        }


        public decimal CalculateNetWorkTime(decimal grossWorkTimeInMinutes, decimal minWorkHoursForMealBreak, decimal mealTimeInMinutes, decimal breakTimeInMinutes, decimal adjustmentInMinutes)
        {
            decimal netWorkTime = grossWorkTimeInMinutes + adjustmentInMinutes;

            // deduct any other break
            if (netWorkTime > breakTimeInMinutes)
                netWorkTime -= breakTimeInMinutes;

            // deduct meal break
            if (netWorkTime > mealTimeInMinutes && netWorkTime > minWorkHoursForMealBreak * 60)
                netWorkTime -= mealTimeInMinutes;

            return netWorkTime;
        }

        #endregion


        #region CaptureCandidateWorkTimeAsync

        public Task<IList<string>> CaptureCandidateWorkTimeAsync(DateTime startDate, DateTime endDate, Account account = null, bool includePlaced = false, bool includeNotPlaced = false)
        {
            return System.Threading.Tasks.Task.Run(() => CaptureCandidateWorkTime(startDate, endDate, account, includePlaced, includeNotPlaced));
        }

        #endregion


        #region ChangeCandidateWorkTimeStatus

        public void ChangeCandidateWorkTimeStatus(CandidateWorkTime canWorkTime, CandidateWorkTimeStatus status, Account currentAccount)
        {
            // update status
            if (canWorkTime.CandidateWorkTimeStatusId != (int)status)
            {
                canWorkTime.CandidateWorkTimeStatusId = (int)status;

                // Lock the work time to prevent further change if approved
                if (status == CandidateWorkTimeStatus.Approved)
                {
                    canWorkTime.ApprovedBy = currentAccount.Id;
                    canWorkTime.ApprovedByName = currentAccount.FullName;
                    canWorkTime.ApprovedOnUtc = DateTime.UtcNow;
                }

                // clear all values for voided work time
                else if (status == CandidateWorkTimeStatus.Voided)
                {
                    //canWorkTime.ClockIn = null;
                    //canWorkTime.ClockOut = null;
                    canWorkTime.LateInTimeInMinutes = 0M;
                    canWorkTime.EarlyOutTimeInMinutes = 0M;
                    canWorkTime.GracePeriodInMinutes = 0M;
                    canWorkTime.OvertimeGracePeriodInMinutes = 0M;
                    canWorkTime.MealTimeInMinutes = 0M;
                    canWorkTime.BreakTimeInMinutes = 0M;
                    canWorkTime.TotalAbsenceTimeInMinutes = 0M;
                    canWorkTime.JobOrderDurationInMinutes = 0M;
                    canWorkTime.JobOrderDurationInHours = 0M;
                    canWorkTime.ClockTimeInMinutes = 0M;
                    canWorkTime.ClockTimeInHours = 0M;
                    canWorkTime.LateOutTimeInMinutes = 0M;
                    canWorkTime.LateOutTimeInHours = 0M;
                    canWorkTime.GrossWorkTimeInMinutes = 0M;
                    canWorkTime.GrossWorkTimeInHours = 0M;
                    canWorkTime.NetWorkTimeInMinutes = 0M;
                    canWorkTime.NetWorkTimeInHours = 0M;
                }
                else if (status == CandidateWorkTimeStatus.Rejected)
                {
                    canWorkTime.ApprovedBy = currentAccount.Id;
                    canWorkTime.ApprovedByName = currentAccount.FullName;
                }
                else
                {
                    canWorkTime.ApprovedBy = 0;
                }

                canWorkTime.UpdatedOnUtc = DateTime.UtcNow;
                canWorkTime.UpdatedBy = currentAccount.Id;

                UpdateCandidateWorkTime(canWorkTime);
            }
        }

        #endregion


        #region Daily Approval Support

        public bool ManualAdjustCandidateWorkTime(int candidateId, int jobOrderId, DateTime startDate, int newJobOrderId, decimal adjustmentMinutes, decimal netHours, string note)
        {
            bool result = false;

            CandidateWorkTime candidateWorkTime = GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, jobOrderId, startDate);

            // adjust existing worktime
            if (newJobOrderId == jobOrderId)
            {
                if (candidateWorkTime == null)
                {
                    _logger.Error(String.Format("ManualAdjustCandidateWorkTime(): Time sheet does not exist. Parameters: candidateId={0}  jobOrderId={1}  startDate={2}", candidateId, jobOrderId, startDate));
                    return false;
                }

                // skip Approved or Voided worktime
                //if (candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved ||
                //    candidateWorkTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Voided)
                //    throw new InvalidOperationException("Worktime is approved or voided.");

                candidateWorkTime.Source = WorkTimeSource.Manual;
                if (netHours > 0)
                {
                    candidateWorkTime.NetWorkTimeInHours = netHours;
                    candidateWorkTime.NetWorkTimeInMinutes = Math.Round(candidateWorkTime.NetWorkTimeInHours * 60);
                    candidateWorkTime.AdjustmentInMinutes = Math.Round(candidateWorkTime.NetWorkTimeInMinutes - candidateWorkTime.GrossWorkTimeInMinutes +
                                                                       candidateWorkTime.BreakTimeInMinutes + candidateWorkTime.MealTimeInMinutes);
                }
                else
                {
                    candidateWorkTime.AdjustmentInMinutes = adjustmentMinutes;
                    candidateWorkTime.NetWorkTimeInMinutes = Math.Round(candidateWorkTime.GrossWorkTimeInMinutes + candidateWorkTime.AdjustmentInMinutes);
                    candidateWorkTime.NetWorkTimeInHours = CommonHelper.RoundUp(candidateWorkTime.NetWorkTimeInMinutes / 60, WORKTIME_DECIMAL_PLACES);      // always round up
                }

                candidateWorkTime.Note = note;
                candidateWorkTime.UpdatedOnUtc = DateTime.UtcNow;
                candidateWorkTime.UpdatedBy = _workContext.CurrentAccount.Id;

                UpdateCandidateWorkTime(candidateWorkTime);

                _activityLogService.InsertActivityLog("SaveAdjustedWorkTime", String.Concat("Adjusted Time Sheet for candidate ", candidateId, " Job Order ", jobOrderId, " on ", startDate));

                result = true;
            }

            // Add worktime to new job order
            else
            {
                CandidateWorkTime newCandidateWorkTime = GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, newJobOrderId, startDate);

                if (newCandidateWorkTime != null)
                {
                    _logger.Error(String.Format("ManualAdjustCandidateWorkTime(): The new job jorder has some time sheets. Parameters: candidateId={0}  jobOrderId={1}  startDate={2}", candidateId, newJobOrderId, startDate));
                    return false;
                }

                JobOrder jobOrder = _jobOrderService.GetJobOrderById(newJobOrderId);

                // skill null or closed job order
                if (jobOrder == null || jobOrder.JobOrderStatusId == (int)JobOrderStatusEnum.Closed)
                {
                    _logger.Error(String.Format("ManualAdjustCandidateWorkTime(): Job order is invalid or closed. Parameters: jobOrderId={0} ", newJobOrderId));
                    return false;
                }


                // make sure job order is not full
                int placedCandidateJO = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(newJobOrderId, startDate)
                                        .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed).Count();
                if ((jobOrder.OpeningNumber - placedCandidateJO) < 1)
                {
                    _logger.Error(String.Format("ManualAdjustCandidateWorkTime(): Job order is full. Parameters: jobOrderId={0} ", newJobOrderId));
                    return false;
                }

                int enteredBy = 0;
                if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                    enteredBy = _workContext.CurrentAccount.Id;

                // place the candiate in the new job order
                _candidateJobOrderService.InsertOrUpdateCandidateJobOrder(newJobOrderId, candidateId, startDate, (int)CandidateJobOrderStatusEnum.Placed, startDate, enteredBy);

                // add manual work time for the new job order
                newCandidateWorkTime = new CandidateWorkTime();

                newCandidateWorkTime.CandidateId = candidateId;
                newCandidateWorkTime.JobOrderId = newJobOrderId;
                newCandidateWorkTime.JobStartDateTime = startDate;
                newCandidateWorkTime.NetWorkTimeInMinutes = candidateWorkTime.NetWorkTimeInMinutes + (adjustmentMinutes - candidateWorkTime.AdjustmentInMinutes);
                newCandidateWorkTime.NetWorkTimeInHours = CommonHelper.RoundUp(newCandidateWorkTime.NetWorkTimeInMinutes / 60, WORKTIME_DECIMAL_PLACES);      // always round up
                newCandidateWorkTime.AdjustmentInMinutes = adjustmentMinutes;
                newCandidateWorkTime.Note = note;

                SaveManualCandidateWorkTime(newCandidateWorkTime, WorkTimeSource.Manual);

                // void the work time for old job order
                if (candidateWorkTime != null)
                {
                    candidateWorkTime.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.Voided;
                    UpdateCandidateWorkTime(candidateWorkTime);

                    _activityLogService.InsertActivityLog("SaveAdjustedWorkTime", String.Concat("Adjusted Time Sheet for candidate ", candidateId, " Job Order ", candidateWorkTime.JobOrderId, " on ", candidateWorkTime.JobStartDateTime.Date));
                }

                result = true;
            }

            return result;
        }


        public int SaveManualCandidateWorkTime(CandidateWorkTime model, string source, int statusId = (int)CandidateWorkTimeStatus.PendingApproval)
        {
            // assuming this model only contains CandidateId, JobOrderId, JobStart Date, Net hours and Notes

            if (model == null)
                throw new WfmException("CandiateWorkTime model is null");

            var existingWorkTime = this.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(model.CandidateId, model.JobOrderId, model.JobStartDateTime);

            if (existingWorkTime != null)
                throw new WfmException(String.Format("Candidate {0} already has a time sheet for Jor Order {1} on {2}.", model.CandidateId, model.JobOrderId, model.JobStartDateTime));

            JobOrder jobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);
            if (jobOrder == null || jobOrder.JobOrderStatusId != (int)JobOrderStatusEnum.Active)
                throw new WfmException("Job order is invalid or inactive.");

            model.CompanyId = jobOrder.CompanyId;
            model.CandidateWorkTimeStatusId = statusId;
            model.ShiftId = jobOrder.ShiftId;
            model.CompanyLocationId = jobOrder.CompanyLocationId;
            model.CompanyDepartmentId = jobOrder.CompanyDepartmentId;
            model.CompanyContactId = jobOrder.CompanyContactId;
            model.JobStartDateTime = model.JobStartDateTime.Date + jobOrder.StartTime.TimeOfDay;
            model.JobEndDateTime = model.JobStartDateTime.AddDays(jobOrder.EndTime.TimeOfDay < jobOrder.StartTime.TimeOfDay ? 1 : 0).Date + jobOrder.EndTime.TimeOfDay;
            model.Year = CommonHelper.GetYearAndWeekNumber(model.JobStartDateTime, out int weekOfYear);
            model.WeekOfYear = weekOfYear;

            // mark source
            if (source == null || (source != WorkTimeSource.Manual && source != WorkTimeSource.Import))
                source = WorkTimeSource.Manual;
            model.Source = source;

            // model.GrossWorkTimeInHours = model.NetWorkTimeInHours;
            // model.GrossWorkTimeInMinutes = model.GrossWorkTimeInHours * 60;
            model.NetWorkTimeInMinutes = model.NetWorkTimeInHours * 60;
            model.AdjustmentInMinutes = model.NetWorkTimeInHours * 60;
            model.Payroll_BatchId = null;

            model.EnteredBy = _workContext.CurrentAccount.Id;
            model.ApprovedBy = statusId == (int)CandidateWorkTimeStatus.Approved ? model.EnteredBy : 0;
            model.ApprovedByName = statusId == (int)CandidateWorkTimeStatus.Approved ? _workContext.CurrentAccount.FullName : null;
            model.FranchiseId = jobOrder.FranchiseId;
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = model.CreatedOnUtc;
            model.ApprovedOnUtc = statusId == (int)CandidateWorkTimeStatus.Approved ? model.UpdatedOnUtc : null;

            this.InsertCandidateWorkTime(model);

            return model.Id;
        }

        #endregion


        #region Alert missing clock in/out

        public void AlertMissingClockInOut()
        {
            //get schedule task running intervals
            var scheduleTask = _scheduleTaskService.GetTaskByType("Wfm.Services.JobOrders.AlertMissingClockInOutTask, Wfm.Services");
            int intervalInSeconds = scheduleTask.Seconds;

            // default grace periond in minutes
            int defaultGracePeriod = 12;

            // checkpoint date time (till now, after last run)
            var checkEndDT = DateTime.Now;
            var checkStartDT = checkEndDT.AddSeconds(-intervalInSeconds);

            #region All Job Orders during the time frame

            var jobOrderList = _jobOrderService.GetJobOrdersByDateAndTimeRange(checkStartDT, checkEndDT, defaultGracePeriod);

            foreach (var jobOrder in jobOrderList)
            {
                // Retrieve clock devices by location
                var clockDevices = _clockDeviceService.GetClockDevicesByCompanyLocationId(jobOrder.CompanyLocationId);
                if (clockDevices.Count == 0)  // If the location has no punch clocks, skip the check for the job order
                    continue;

                #region Define punch time Scanning window

                // *** Scan clock time ***
                DateTime startTime = jobOrder.StartTime;
                DateTime endTime = jobOrder.EndTime;

                var startDate = checkStartDT.Date;
                var endDate = checkStartDT.Date.AddDays(startTime.TimeOfDay > endTime.TimeOfDay ? 1 : 0);

                // Define job start date time / end date time
                DateTime jobStartDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hour, startTime.Minute, 0);
                DateTime jobEndDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endTime.Hour, endTime.Minute, 0);

                // *** Define scan time window ***
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // we only scan the clock time 1 hour before and 1 hour after job start time
                TimeSpan scanWindowSpan = new TimeSpan(0, 0, _candidateWorkTimeSettings.StartScanWindowSpanInMinutes, 0);
                DateTime scanStartDateTime = jobStartDateTime - scanWindowSpan;
                DateTime scanEndDateTime = jobEndDateTime + scanWindowSpan;

                #endregion

                #region All "PLACED" Candidates in this JobOrder

                var canJoStatusList = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndDateRange(jobOrder.Id, DateTime.Today, DateTime.Today).Select(cj => cj.CandidateId).Distinct();
                var missedClockingCandidateList = new List<Candidate>(); // to store missing clocking candidate

                foreach (var candiateId in canJoStatusList)
                {
                    var candidateClockTimes = _clockTimeService.GetAllClockTimesByCandidateIdAndCompanyIdAndDateTimeRange(candiateId, jobOrder.CompanyId, scanStartDateTime, scanEndDateTime);
                    if (candidateClockTimes.Count == 0)
                    {
                        var candidate = _candidateService.GetCandidateById(candiateId);
                        missedClockingCandidateList.Add(candidate);
                    }
                }

                #endregion // All Candidates in the job order

                #region Alert Recruitor
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (missedClockingCandidateList.Count > 0)
                {
                    var company = _companyService.GetCompanyByIdForScheduleTask(jobOrder.CompanyId);
                    var contact = _companyContactService.GetCompanyContactById(jobOrder.CompanyContactId);
                    //if(contact!=null)
                    _workflowMessageService.SendCandidateMissedClockingRecruiterNotification(company, jobOrder, contact, missedClockingCandidateList, 1);
                }

                #endregion
            }
            #endregion // All job orders in the period

        }

        #endregion


        #region Candidate Attendance

        public IList<int> GetJobOrderPlacedCandidateIds(int jobOrderId, DateTime startDate)
        {
            var jo = _jobOrderService.GetJobOrderById(jobOrderId);
            var endDate = startDate.AddDays(6);
            return jo.CandidateJobOrders.Where(x => x.StartDate <= endDate && (!x.EndDate.HasValue || x.EndDate >= startDate) &&
                                                    x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                                        .Select(x => x.CandidateId)
                                        .Distinct()
                                        .ToList();
        }


        public IList<int>[] GetAttendanceCountByJobOrderAndSelectedWeek(int jobOrderId, DateTime startDate)
        {
            var attendanceCounters = new List<int>[] {
                new List<int> {0, 0}, 
                new List<int> {0, 0}, 
                new List<int> {0, 0}, 
                new List<int> {0, 0}, 
                new List<int> {0, 0}, 
                new List<int> {0, 0}, 
                new List<int> {0, 0}
            };

            for (int i = 0; i < 7; i++)
            {
                var jobOrderWorkTimes = GetAllWorkTimeByJobOrderAndDateAsQueryable(jobOrderId, startDate.AddDays(i));
                attendanceCounters[i][0] = jobOrderWorkTimes.Distinct().Count();                                            // punched
                attendanceCounters[i][1] = jobOrderWorkTimes.Where(x => x.NetWorkTimeInMinutes >= 3).Distinct().Count();    // valid work time
            }

            return attendanceCounters;
        }

        #endregion


        #region time sheet approval

        public IQueryable<TimeSheetSummary> GetTimeSheetSummaryForApprovalByWeekStartDate(DateTime weekStartDate)
        {
            var year = CommonHelper.GetYearAndWeekNumber(weekStartDate, out int weekOfYear);
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("Year", year);
            paras[1] = new SqlParameter("WeekOfYear", weekOfYear);

            var result = _dbContext.SqlQuery<TimeSheetSummary>("EXEC [dbo].[GetTimeSheetSummary] @Year, @WeekOfYear", paras)
                         .Where(x => x.SubmittedHours > 0 || x.ApprovedHours > 0)
                         .AsQueryable();
            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
                result = result.Where(x => x.VendorId == account.FranchiseId);
            return result.OrderBy(x => x.CompanyId);
        }


        public IQueryable<TimeSheetSummary> GetTimeSheetSummaryForApprovalByDateRange(DateTime startDate, DateTime endDate)
        {
            var paras = new SqlParameter[]
                {
                    new SqlParameter("StartDate", startDate),
                    new SqlParameter("EndDate", endDate)
                };

            var result = _dbContext.SqlQuery<TimeSheetSummary>("EXEC [dbo].[GetTimeSheetSummaryByDateRange] @StartDate, @EndDate", paras)
                         .Where(x => x.SubmittedHours > 0 || x.ApprovedHours > 0).AsQueryable();

            var account = _workContext.CurrentAccount;
            result = result.Where(x => !account.IsVendor() || x.VendorId == account.FranchiseId);

            return result.OrderBy(x => x.CompanyId);
        }


        public void ApproveWorkTimeByCompanyIdAndDateRange(int companyId, int? supervisorId, DateTime startDate, DateTime endDate, int vendorId = 0)
        {
            var query = _workTimeRepository.Table;

            query = from t in query
                    where t.CompanyId == companyId &&
                          DbFunctions.TruncateTime(t.JobStartDateTime) >= startDate && DbFunctions.TruncateTime(t.JobStartDateTime) <= endDate &&
                          t.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval &&
                          t.NetWorkTimeInHours > 0
                    select t;

            if (vendorId > 0)
                query = query.Where(t => t.FranchiseId == vendorId);

            if (supervisorId != null)
                query = query.Where(x => x.CompanyContactId == supervisorId.Value);
            var workTimes = query.ToList();

            workTimes.ForEach(x =>
            {
                x.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.Approved;
                x.ApprovedBy = _workContext.CurrentAccount.Id;
                x.ApprovedByName = _workContext.CurrentAccount.FullName;
                x.ApprovedOnUtc = DateTime.UtcNow;
                x.UpdatedOnUtc = DateTime.UtcNow;
            });

            _dbContext.SaveChanges();

            // cleanup matched work times
            foreach (var cwt in workTimes)
            {
                if (IsValidWorkTime(cwt))
                {
                    RemoveOtherMatchedWorkTimes(cwt);
                    SetClockTimeStatusByWorkTime(cwt);
                }
            }
        }


        public int[] GetSupervisorIdsWithPendingApproval(DateTime startDate, DateTime endDate, int? companyId)
        {
            var query = _workTimeRepository.Table;

            query = query.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval &&
                                     DbFunctions.TruncateTime(x.JobStartDateTime) >= startDate &&
                                     DbFunctions.TruncateTime(x.JobStartDateTime) <= endDate);

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId.Value);

            return query.Select(x => x.CompanyContactId).Distinct().ToArray();
        }

        #endregion


        #region Invoice

        public void SetWorkTimeInvoiceDate(int companyId, DateTime startDate, DateTime endDate)
        {
            var query = _workTimeRepository.Table;

            query = from t in query
                    where t.CompanyId == companyId &&
                          DbFunctions.TruncateTime(t.JobStartDateTime) >= startDate && DbFunctions.TruncateTime(t.JobStartDateTime) <= endDate &&
                          t.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved
                    select t;

            query.ToList().ForEach(x =>
            {
                x.InvoiceDate = DateTime.UtcNow;
                x.UpdatedOnUtc = DateTime.UtcNow;
            });

            _dbContext.SaveChanges();
        }

        public async Task SetWorkTimeInvoiceDate(string selectedIds)
        {
            var ids = selectedIds.Split(new char[] { ',' }).Select(x => Convert.ToInt32(x));
            var query = _workTimeRepository.Table.Where(x => ids.Contains(x.Id));
            await query.ForEachAsync(x => { x.InvoiceDate = DateTime.UtcNow; x.UpdatedOnUtc = DateTime.UtcNow; });
            _dbContext.SaveChanges();
        }
        #endregion


        #region Client Timesheet Document

        public void InsertClientTimeSheetDocument(ClientTimeSheetDocument clientTimeSheetDoc)
        {
            if (clientTimeSheetDoc == null)
                throw new ArgumentNullException("clientTimeSheetDoc");

            _clientTimeSheetDocumentRepositoty.Insert(clientTimeSheetDoc);
        }


        public void DeleteClientTimeSheetDocument(ClientTimeSheetDocument clientTimeSheetDoc)
        {
            if (clientTimeSheetDoc == null)
                throw new ArgumentNullException("clientTimeSheetDoc");

            _clientTimeSheetDocumentRepositoty.Delete(clientTimeSheetDoc);
        }


        public ClientTimeSheetDocument GetClientTimeSheetDocumentById(int id)
        {
            if (id == 0)
                return null;

            return _clientTimeSheetDocumentRepositoty.GetById(id);
        }


        public IList<ClientTimeSheetDocument> GetAllClientTimeSheetDocumentsByJobOrderIdAndDateRange(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var query = _clientTimeSheetDocumentRepositoty.Table;

            query = from c in query
                    where c.JobOrderId == jobOrderId && c.StartDate >= startDate && c.EndDate <= endDate
                    select c;

            return query.ToList();
        }

        #endregion


        #region CheckApprovedWorkTime

        public bool CheckApprovedWorkTime(int candidateId, int jobOrderId, DateTime time)
        {
            List<CandidateWorkTime> worktimes = _workTimeRepository.Table.Where(x => x.CandidateId == candidateId && x.JobOrderId == jobOrderId && DbFunctions.TruncateTime(x.JobStartDateTime) == time.Date).ToList();
            foreach (CandidateWorkTime worktime in worktimes)
            {
                if (worktime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved)
                    return true;
            }
            return false;
        }


        public bool CheckApprovedWorkTimeWithinDateRange(int candidateId, int jobOrderId, DateTime startDate, DateTime? endDate = null)
        {
            bool result = _workTimeRepository.Table.Where(x => x.CandidateId == candidateId &&
                                                               x.JobOrderId == jobOrderId &&
                                                               DbFunctions.TruncateTime(x.JobStartDateTime) >= startDate &&
                                                               (!endDate.HasValue || DbFunctions.TruncateTime(x.JobStartDateTime) <= endDate.Value) &&
                                                               x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved
                          ).Any();

            return result;
        }


        public bool IfAllJobOrderWorkTimeApproved(IList<int> jobOrderIds, DateTime refDate)
        {
            refDate = refDate.AddDays(1);
            var query = _workTimeRepository.Table
                        .Where(x => x.JobStartDateTime < refDate)
                        .Where(x => jobOrderIds.Contains(x.JobOrderId))
                        .Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval);

            return !query.Any();
        }

        #endregion

        
        #region Get Total Hours the candidate worked for the company

        public decimal GetCandidateTotalHoursForCompany(int candidateId, int companyId)
        {
            var query = GetWorkTimeByCandidateIdAsQueryable(candidateId);
            query = query.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved && x.CompanyId == companyId);
            if (query.Count() > 0)
            {
                decimal total = query.Select(x => x.NetWorkTimeInHours).Sum();
                return total;
            }
            else
                return 0;
        }


        public IQueryable<EmployeeTotalHours> GetAllEmployeeTotalHours(DateTime startDate, DateTime endDate, int[] status, int companyId = 0, Account account = null)
        {
            var candidates = _candidateService.GetAllCandidatesAsQueryable(account, true, true, true);
            var hours = this.GetAllCandidateWorkTimeAsQueryable(isAccountBased: false)
                        .Where(x => (companyId == 0 || x.CompanyId == companyId) && status.Contains(x.CandidateWorkTimeStatusId))
                        .Where(x => DbFunctions.TruncateTime(x.JobStartDateTime) >= startDate && DbFunctions.TruncateTime(x.JobStartDateTime) <= endDate)
                        .GroupBy(x => x.CandidateId)
                        .Select(g => new
                        {
                            CandidateId = g.Key,
                            TotalHours = g.Sum(x => x.NetWorkTimeInHours)
                        });

            var result = hours.Join(candidates, h => h.CandidateId, c => c.Id, (h, c) => new EmployeeTotalHours()
            {
                CandidateId = h.CandidateId,
                FranchiseId = c.FranchiseId,
                EmployeeId = c.EmployeeId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                HomePhone = c.HomePhone,
                MobilePhone = c.MobilePhone,
                TotalHours = h.TotalHours
            });

            return result;
        }

        #endregion
    }
}
