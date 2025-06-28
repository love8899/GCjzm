using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Helpers;
using Wfm.Services.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.ClockTime;
using System.Data.SqlClient;
using Wfm.Data;
using Wfm.Core.Domain.Common;
using Wfm.Services.Companies;


namespace Wfm.Services.JobOrders
{
    public partial class JobOrderService : IJobOrderService
    {
        #region Fields

        private readonly IGenericHelper _genericHelper;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<JobOrderOpening> _jobOrderOpeningRepository;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IRepository<CandidateClockTime> _clockTimeRepository;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IRepository<CompanyDepartment> _companyDepartmentRepository;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IRepository<City> _cityRepository;
        private readonly IDbContext _dbContext;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyVendorService _companyVendorService;

        #endregion

        #region Ctor

        public JobOrderService(
            IGenericHelper genericHelper,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<JobOrderOpening> jobOrderOpeningRepository,
            IRepository<CompanyDepartment> companyDepartmentRepository,
            IRepository<CompanyLocation> companyLocationRepository,
            IRepository<CandidateClockTime> clockTimeRepository,
            ICandidateJobOrderService candidateJobOrderService,
            ICompanyBillingService companyBillingService,
            IRepository<City> cityRepository,
            IDbContext dbContext,
            IRecruiterCompanyService recruiterCompanyService,
            IWorkContext workContext,
            ICompanyVendorService companyVendorService)
        {
            _genericHelper = genericHelper;
            _jobOrderRepository = jobOrderRepository;
            _jobOrderOpeningRepository = jobOrderOpeningRepository;
            _companyLocationRepository = companyLocationRepository;
            _clockTimeRepository = clockTimeRepository;
            _candidateJobOrderService = candidateJobOrderService;
            _companyDepartmentRepository = companyDepartmentRepository;
            _companyBillingService = companyBillingService;
            _cityRepository = cityRepository;
            _dbContext = dbContext;
            _recruiterCompanyService = recruiterCompanyService;
            _workContext = workContext;
            _companyVendorService = companyVendorService;
        }

        #endregion

        #region CRUD

        public void InsertJobOrder(JobOrder jobOrder)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            jobOrder.CreatedOnUtc = DateTime.UtcNow;
            jobOrder.UpdatedOnUtc = DateTime.UtcNow;
         
            _jobOrderRepository.Insert(jobOrder);
        }

        public void UpdateJobOrder(JobOrder jobOrder)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            jobOrder.UpdatedOnUtc = DateTime.UtcNow;
            _jobOrderRepository.Update(jobOrder);
        }

        #endregion

        #region JobOrder

        public JobOrder GetJobOrderById(int id)
        {
            if (id == 0)
                return null;

            return _jobOrderRepository.Table
                .Include(x => x.Company)
                .Include(x => x.Shift)
                .Where(x => x.Id == id).FirstOrDefault();
        }

        public JobOrder GetJobOrderByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            return _jobOrderRepository.Table.Where(x => x.JobOrderGuid == guid).FirstOrDefault();
        }

        public JobOrder CopyJobOrder(JobOrder jobOrderCopy, Guid? origJobOrderGuid)
        {
            if (jobOrderCopy == null || origJobOrderGuid == null)
                throw new ArgumentNullException("jobOrderCopy");

            // add new job order
            InsertJobOrder(jobOrderCopy);

            var jobOrderOrig = GetJobOrderByGuid(origJobOrderGuid);
            if (jobOrderOrig != null && jobOrderOrig.CompanyId == jobOrderCopy.CompanyId)
            {
                // Add original candidates to the NEW pipeline
                // Add original candidates to NEW job order status History
                var refDate = DateTime.Today;
                var candidateJobOrders = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(jobOrderOrig.Id, refDate);

                foreach (var item in candidateJobOrders)
                {
                    var newCandidateJobOrder = new CandidateJobOrder()
                    {
                        CandidateId = item.CandidateId,
                        JobOrderId = jobOrderCopy.Id, // new job order id
                        StartDate = jobOrderCopy.StartDate,
                        CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.NoStatus,
                        RatingValue = 0,
                        RatingComment = null,
                        HelpfulNoTotal = 0,
                        HelpfulYesTotal = 0,
                        RatedBy = null,
                        EnteredBy = _workContext.CurrentAccount.Id,
                        Note = null,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    // Add to the new pipeline
                    _candidateJobOrderService.InsertCandidateJobOrder(newCandidateJobOrder);

                }
            }

            return jobOrderCopy;
        }

        public JobOrder GetJobOrderByJobPostingIdAndVendorId(int jobPostingId, int vendorId)
        {
            return _jobOrderRepository.Table.Where(x => x.JobPostingId == jobPostingId && x.FranchiseId == vendorId &&
                                                        x.JobOrderStatusId == (int)JobOrderStatusEnum.Active && x.IsDeleted == false)
                   .FirstOrDefault();
        }

        public bool JobOrderHasWorkTimeInDateRange(int jobOrderId, DateTime from, DateTime to)
        {
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("JobOrderId", jobOrderId);
            paras[1] = new SqlParameter("from", from);
            paras[2] = new SqlParameter("to", to);

            const string query =
                @"Select top 1 1 as Found 
                  Where Exists (Select Id from CandidateWorkTime
                                where jobOrderId = @JobOrderId and
			                    ((JobStartDateTime between @from and @to) OR
				                  (JobEndDateTime between @from and @to)
				                )
			     )";
            var result = _dbContext.SqlQuery<int>(query, paras);

            return result.Count<int>() > 0;
        }

        public bool PipelineHasDataInDateRange(int jobOrderId, DateTime from, DateTime to)
        {
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("JobOrderId", jobOrderId);
            paras[1] = new SqlParameter("from", from);
            paras[2] = new SqlParameter("to", to);

            const string query =
                @"Select top 1 1 as Found
                  Where Exists (Select 1 from CandidatejobOrder
                      where jobOrderId = @JobOrderId and CandidateJobOrderStatusId = 12 and 
			         (
				       (EndDate is null and StartDate <= @to) OR
				       (EndDate between @from and @to)
				     )
			      )";
            var result = _dbContext.SqlQuery<int>(query, paras);

            return result.Count<int>() > 0;
        }

        public void UpdatePipeLineDateRange(int jobOrderId, DateTime to)
        {
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("JobOrderId", jobOrderId);
            paras[1] = new SqlParameter("to", to);

            const string query =
                @"Delete CandidateJobOrder Where JobOrderId = @jobOrderId and EndDate is not null and EndDate < @to 

                  Update CandidateJobOrder
                  Set StartDate = @to, UpdatedOnUtc = GETUTCDATE()
                  Where JobOrderId = @jobOrderId and StartDate < @to and CandidateJobOrderStatusId <> 12
                 ";
            _dbContext.ExecuteSqlCommand(query, false, null, paras);
        }

        #endregion

        #region PagedList

        // Web helper methods
        //----------------------------------

        public IList<JobOrder> GetLastPublishedJobOrders(int jobOrderCategoryId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _jobOrderRepository.Table;

            // published and active
            query = query.Where(jo => jo.IsPublished == true && jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            // specific category
            if (jobOrderCategoryId > 0)
                query = query.Where(jo => jo.JobOrderCategoryId == jobOrderCategoryId);

            // show deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);


            query = from jo in query
                    orderby (jo.UpdatedOnUtc) descending
                    select jo;

            var jobOrders = new PagedList<JobOrder>(query, pageIndex, pageSize);

            return jobOrders;
        }

        // Web search
        public IPagedList<JobOrder> GetAllPublishedJobOrders(string searchString = null, int jobOrderCategoryId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _jobOrderRepository.Table;

            // published and active
            query = query.Where(jo => jo.IsPublished == true && jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            // specific category
            if (jobOrderCategoryId > 0)
                query = query.Where(jo => jo.JobOrderCategoryId == jobOrderCategoryId);

            // show deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);


            // search key words
            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(jo =>
                        jo.Id.ToString() == searchString ||
                        jo.JobTitle.Contains(searchString) ||
                        jo.JobDescription.Contains(searchString)
                        );

            // sort
            query = from jo in query
                    orderby jo.IsHot, jo.UpdatedOnUtc descending
                    select jo;

            var jobOrders = new PagedList<JobOrder>(query, pageIndex, pageSize);

            return jobOrders;
        }


        // admin helper methods
        //----------------------------------

        public IPagedList<JobOrder> GetAllJobOrders(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _jobOrderRepository.Table;

            // deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);

            query = from jo in query
                    orderby jo.UpdatedOnUtc descending
                    select jo;

            var jobOrders = new PagedList<JobOrder>(query, pageIndex, pageSize);

            return jobOrders;
        }


        /// <summary>
        /// Gets all job orders asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IQueryable<JobOrder> GetAllJobOrdersAsQueryable(Account account)
        {
            if (account == null)
                return null;

            var query = _jobOrderRepository.Table;

          

            //// active
            //if (!showInactive)
            //    query = query.Where(jo => jo.IsActive == true);
            //// client
            //if (!showClient)
            //    query = query.Where(jo => jo.IsClientAccount == false);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(jo => jo.IsDeleted == false);

            // IsLimitedToFranchises
            if (account.IsLimitedToFranchises)
                query = query.Where(jo => jo.FranchiseId == account.FranchiseId);

            query = from jo in query
                    where jo.IsDeleted == false
                    orderby jo.UpdatedOnUtc descending
                    select jo;


            return query.AsQueryable();
        }


        public IQueryable<JobOrder> GetAllJobOrdersByCompanyIdAsQueryable(int companyId, bool showHidden = false)
        {
            var query = _jobOrderRepository.Table;

            // Exclude Direct hire job orders
            query = query.Where(j => !j.JobOrderType.IsDirectHire);

            // show deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);
            if (companyId != 0)
                query = query.Where(x => x.CompanyId == companyId);

            query = from jo in query
                    orderby jo.UpdatedOnUtc descending
                    select jo;


            return query.AsQueryable();
        }


        public IQueryable<JobOrder> GetAllJobOrdersByJobPostingIdAsQueryable(Account account, int jobPostingId)
        {
            var query = _jobOrderRepository.Table;

            query = from jo in query
                    where jo.JobPostingId == jobPostingId
                    orderby jo.FranchiseId
                    select jo;

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.FranchiseId == account.FranchiseId);

            return query.AsQueryable();
        }


        public IList<JobOrder> GetJobOrdersByAccountAndCompany(Account account, int companyId, DateTime refDate)
        {
            refDate = refDate > DateTime.MinValue ? refDate : DateTime.Today;
            var jobOrders = GetAllJobOrdersByCompanyIdAsQueryable(companyId)
                            .Where(x => x.JobOrderStatusId == (int)JobOrderStatusEnum.Active &&
                                        x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate >= refDate));
            if (!account.IsClient())
            {
                if (account.IsLimitedToFranchises)
                    jobOrders = jobOrders.Where(x => x.FranchiseId == account.FranchiseId);
                if (account.IsRecruiterOrRecruiterSupervisor())
                    jobOrders = jobOrders.Where(x => x.OwnerId == account.Id || x.RecruiterId == account.Id);
            }
            return jobOrders.ToList();
        }


        public IList<AttendancePreview> GetAttendancePreviewByDate(DateTime start, DateTime end, int? companyId)
        {
            Account account = _workContext.CurrentAccount;
            List<int> companyIds = new List<int>();
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@startDate", start);
            paras[1] = new SqlParameter("@endDate", end);
            paras[2] = new SqlParameter("@companyId", companyId == null ? 0 : companyId);
            List<int> Ids = _jobOrderRepository.TableNoTracking.Where(x => x.FranchiseId == _workContext.CurrentFranchise.Id).Select(x => x.Id).ToList();
            var result = _dbContext.SqlQuery<AttendancePreview>("EXEC [dbo].[AttendancePreview] @startDate, @endDate,@companyId", paras).ToList();
            if (account.IsVendor())
            {
                var companies = _companyVendorService.GetAllCompaniesByVendorId(account.FranchiseId).Select(x => x.CompanyId);
                result = result.Where(x => Ids.Contains(x.JobOrderId)&&companies.Contains(x.CompanyId)).ToList();
                if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                {
                    companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                    result = result.Where(x => companyIds.Contains(x.CompanyId)).ToList();
                }
            }
            if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor())
            {
                companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                result = result.Where(x => companyIds.Contains(x.CompanyId)).ToList();
            }
            return result;
        }

        // Time sheet helper methods
        //----------------------------------
        public IQueryable<JobOrder> GetJobOrdersByDateRangeAsQueryable(DateTime startDate, DateTime endDate,bool includeDirectHire=false)
        {
            if ((startDate > endDate) || endDate == null || startDate == null)
                return null;

            var query = _jobOrderRepository.Table;

            // Exclude Direct hire job orders
            if(!includeDirectHire)
                query = query.Where(j => !j.JobOrderType.IsDirectHire);

            // Only active job order
            query = query.Where(jo => jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            query = from jo in query
                    where (!jo.EndDate.HasValue || jo.EndDate >= startDate) && jo.StartDate <= endDate
                    orderby jo.EndDate.HasValue descending, jo.EndDate, jo.EndTime                          // this is critical for overtime calculation
                    select jo;


            return query.AsQueryable();
        }

        public IQueryable<JobOrder> GetJobOrdersForWorkTimeCalculation(DateTime startDate, DateTime endDate)
        {

            var notProcessedId = (int)(CandidateClockTimeStatus.NoStatus);
            // startDate & endDate containing Date only (i.e. time = 00:00:00.000). Actual date of ClockInOut could be 1 day eralier or later than them.
            var minStartDate = startDate.AddDays(-1);
            var maxEndDate = endDate.AddDays(+1);

            var locationIds = _clockTimeRepository.TableNoTracking.Where(x => x.CandidateClockTimeStatusId == notProcessedId && x.ClockInOut > minStartDate && x.ClockInOut < maxEndDate)
                .Select(x => x.CompanyLocationId).Distinct().ToArray();

            var ids = GetJobOrdersByDateRangeAsQueryable(startDate, endDate)
                .Join(_companyLocationRepository.TableNoTracking
                , x => x.CompanyLocationId, y => y.Id, (x, y) => new
                {
                    JobOrderId = x.Id,
                    CompanyLocationId = y.Id,
                    CandidateJobOrderUpdatedOnUtc = x.CandidateJobOrders.Max(cj => cj.UpdatedOnUtc),
                    LastPunchClockFileUploadDateTimeUtc = y.LastPunchClockFileUploadDateTimeUtc,
                    LastWorkTimeCalculationDateTimeUtc = y.LastWorkTimeCalculationDateTimeUtc,
                    JobOrderUpdatedOnUtc = x.UpdatedOnUtc
                })
                .Where(t => locationIds.Contains(t.CompanyLocationId) || // not processed clock time
                        (!t.LastWorkTimeCalculationDateTimeUtc.HasValue && t.LastPunchClockFileUploadDateTimeUtc.HasValue)    // first time calculation, file uploaded
                        ||
                        (t.LastWorkTimeCalculationDateTimeUtc.HasValue &&                                                       // re-calculation
                            (
                                (t.LastPunchClockFileUploadDateTimeUtc.HasValue &&
                                    t.LastPunchClockFileUploadDateTimeUtc > t.LastWorkTimeCalculationDateTimeUtc)        // new upload, after last calculation
                                ||
                                (t.JobOrderUpdatedOnUtc.HasValue &&
                                    t.JobOrderUpdatedOnUtc > t.LastWorkTimeCalculationDateTimeUtc)       // job order added or updated, after last calculation
                                ||
                                (t.CandidateJobOrderUpdatedOnUtc.HasValue &&
                                    t.CandidateJobOrderUpdatedOnUtc > t.LastWorkTimeCalculationDateTimeUtc)     // candidate job order added or updated, after last calculation
                            )
                        )
                    )
                .Select(t => t.JobOrderId).ToArray();

            return _jobOrderRepository.TableNoTracking.Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Id).ThenByDescending(x => x.EndDate).ThenBy(x => x.EndTime);
        }


        // for missing punch alert (check by start time only!!!)
        public IList<JobOrder> GetJobOrdersByDateAndTimeRange(DateTime startDateTime, DateTime endDateTime, int defaultGracePeriod)
        {
            if ((startDateTime > endDateTime) || endDateTime == null || startDateTime == null)
                return null;

            var paras = new SqlParameter[]
            {
                new SqlParameter("StartDateTime", startDateTime),
                new SqlParameter("EndDateTime", endDateTime),
                new SqlParameter("DefaultGracePeriod", defaultGracePeriod)
            };
            string query = @";with CTE as (
	                            select jo.* from JobOrder as jo
	                            inner join JobOrderStatus jos on jos.Id = jo.JobOrderStatusId
	                            left join SchedulePolicy as sp on sp.Id = jo.SchedulePolicyId
	                            left join RoundingPolicy as rp on rp.Id = sp.RoundingPolicyId
	                            where
		                            (
			                            (DATENAME(DW, @StartDateTime) = 'Sunday' and SundaySwitch = 1) or
			                            (DATENAME(DW, @StartDateTime) = 'Monday' and MondaySwitch = 1) or
			                            (DATENAME(DW, @StartDateTime) = 'Tuesday' and TuesdaySwitch = 1) or 
			                            (DATENAME(DW, @StartDateTime) = 'Wednesday' and WednesdaySwitch = 1) or 
			                            (DATENAME(DW, @StartDateTime) = 'Thursday' and ThursdaySwitch = 1) or 
			                            (DATENAME(DW, @StartDateTime) = 'Friday' and FridaySwitch = 1) or 
			                            (DATENAME(DW, @StartDateTime) = 'Saturday' and SaturdaySwitch = 1)
		                            ) and 
		                            jos.JobOrderStatusName='Active' and 
		                            (
			                            (jo.EndDate is null or jo.EndDate >= cast(@StartDateTime as date)) and jo.StartDate <= cast(@EndDateTime as date) and 
			                            dateadd(mi, isnull(rp.GracePeriodInMinutes, @DefaultGracePeriod), 
					                            cast(cast(@StartDateTime as date) as datetime) + cast(cast(jo.StartTime as time) as datetime))
				                            between @StartDateTime and @EndDateTime
		                            )
                            )

                            Select * from CTE

                            Except
		
                            select jo.*
                            from CTE jo
                            left join CompanyLocation cl on cl.Id = jo.CompanyLocationId
                            left join StateProvince sp on sp.Id = cl.StateProvinceId
                            left join StatutoryHoliday sh on sh.StateProvinceId = sp.Id
                            where jo.IncludeHolidays = 0 and cast(@StartDateTime as date) = sh.HolidayDate
                            order by StartDate, StartTime                            
                    ";
            
            return _jobOrderRepository.GetAllByQuery(query, paras).ToList();
        }


        public IQueryable<JobOrder> GetAllJobOrdersByCompanyIdAsQueryable(Account account, bool showInactive = false, bool showHidden = false)
        {
            var query = _jobOrderRepository.Table;


            // query within Franchise
            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(jo => jo.FranchiseId == account.FranchiseId);

            // query within company
            query = query.Where(jo => jo.CompanyId == account.CompanyId);

            // active
            if (!showInactive)
                query = query.Where(jo => jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            // deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager() || account.IsAdministrator()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId); // search within locatin
            else if (account.IsCompanyDepartmentManager())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId &&
                    jo.CompanyDepartmentId > 0 &&
                    jo.CompanyDepartmentId == account.CompanyDepartmentId);
            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId &&
                    jo.CompanyDepartmentId > 0 &&
                    jo.CompanyDepartmentId == account.CompanyDepartmentId &&    // search within department
                    jo.CompanyContactId == account.Id);
            else
                return query = query.Where(x => false); // No role

            query = from b in query
                    orderby b.StartDate descending, b.StartTime descending
                    select b;

            return query.AsQueryable();
        }

        public IList<JobOrder> GetJobOrdersStartingSoon(DateTime refTime, int min)
        {
            //Get all active job orders based on the refTime
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("refDate", refTime);
            paras[1] = new SqlParameter("min", min);
            string query = @"declare @refDay datetime,@refTime datetime
                            set @refDay = CONVERT(varchar,@refDate,101)
                            set @refTime = CONVERT(varchar,@refDate,108)

                            ;with CTE as (
	                            select jo.* from JobOrder jo
	                            inner join JobOrderStatus jos on jos.Id = jo.JobOrderStatusId
								inner join CandidateJobOrder cjo on cjo.JobOrderId = jo.Id and (cjo.StartDate<=@refDay and (cjo.EndDate is null or cjo.EndDate>=@refDay))
								inner join CandidateJobOrderStatus cjos on cjos.Id = cjo.CandidateJobOrderStatusId and cjos.StatusName = 'Placed'
	                            where (
			                            (DATENAME(DW,@refDate)='Sunday' and SundaySwitch=1) or
			                            (DATENAME(DW,@refDate)='Monday' and MondaySwitch=1) or
			                            (DATENAME(DW,@refDate)='Tuesday' and TuesdaySwitch=1) or 
			                            (DATENAME(DW,@refDate)='Wednesday' and WednesdaySwitch=1) or 
			                            (DATENAME(DW,@refDate)='Thursday' and ThursdaySwitch=1) or 
			                            (DATENAME(DW,@refDate)='Friday' and FridaySwitch=1) or 
			                            (DATENAME(DW,@refDate)='Saturday' and SaturdaySwitch=1)
		                               ) and jos.JobOrderStatusName='Active' and 
		                               (
				                            (jo.EndDate is null or jo.EndDate>=@refDay) and
				                            jo.StartDate<=@refDay and 
				                            (CONVERT(varchar, jo.StartTime,108)>=@refTime) and
				                            (CONVERT(varchar, jo.StartTime,108)<=DATEADD(MINUTE,@min,@refTime))
		                               )
								group by jo.[Id] ,[JobOrderGuid] ,[CompanyId] ,[CompanyLocationId]  ,[CompanyDepartmentId]  ,[CompanyContactId]
									  ,[CompanyJobNumber]  ,[JobTitle]  ,[JobDescription]  ,[HiringDurationExpiredDate]	  ,[EstimatedFinishingDate]
									  ,[EstimatedMargin] ,jo.[StartDate]  ,jo.[EndDate]	  ,[StartTime]  ,[EndTime]  ,[SchedulePolicyId]
									  ,[JobOrderTypeId]  ,[Salary] ,[JobOrderStatusId] ,[JobOrderCategoryId] ,[ShiftId] ,[ShiftSchedule]
									  ,[HoursPerWeek] ,jo.[Note] ,[RequireSafeEquipment] ,[RequireSafetyShoe] ,[IsInternalPosting] ,[IsPublished]
									  ,[IsHot] ,[RecruiterId]  ,[OwnerId] ,jo.[EnteredBy],[FranchiseId],jo.[IsDeleted] ,jo.[CreatedOnUtc]  ,jo.[UpdatedOnUtc]
									  ,[AllowSuperVisorModifyWorkTime],[JobOrderCloseReason] ,[BillingRateCode]  ,[JobPostingId] ,[LabourType] ,[PositionId]
									  ,[MondaySwitch] ,[TuesdaySwitch]  ,[WednesdaySwitch] ,[ThursdaySwitch] ,[FridaySwitch]  ,[SaturdaySwitch] ,[SundaySwitch]
									  ,[IncludeHolidays] ,[SalaryMin],[SalaryMax] ,[FeeMin]  ,[FeeMax],[FeePercent] ,[FeeAmount] ,[FeeTypeId] ,[AllowTimeEntry]
                                      ,[MonsterPostingId]
								having Count(DISTINCT cjo.CandidateId)>0
                            )

                            Select * from CTE


                            Except
		
                            select Distinct jo.*
                            from CTE jo
                            left join CompanyLocation cl on cl.Id = jo.CompanyLocationId
                            left join StateProvince sp on sp.Id = cl.StateProvinceId
                            left join StatutoryHoliday sh on sh.StateProvinceId = sp.Id
                            where jo.IncludeHolidays=0 and @refDay=sh.HolidayDate
                            order by StartDate, StartTime
                        ";
            var jobOrders = _dbContext.SqlQuery<JobOrder>(query,paras).ToList();
            
            //Get all active job order that will start in min Minute
            //var startSoonJobOrders = jobOrders.Where(x => DbFunctions.CreateTime(refTime.Hour, refTime.Minute, refTime.Second) <= DbFunctions.CreateTime(x.StartTime.Hour, x.StartTime.Minute, x.StartTime.Second)
            //                                            && DbFunctions.AddMinutes(refTime.TimeOfDay, min) >= DbFunctions.CreateTime(x.StartTime.Hour, x.StartTime.Minute, x.StartTime.Second)).ToList();
            return jobOrders;
        }


        #endregion


        #region JobOrders Search

        public IList<JobOrder> SearchJobOrders(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false)
        {
            if (String.IsNullOrWhiteSpace(searchKey))
            {
                IList<JobOrder> jobOrders = new List<JobOrder>();
                return jobOrders;
            }

            var query = _jobOrderRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(jo => jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            // deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);

           // Filter only for current vendor
            if (_workContext.CurrentAccount != null && _workContext.CurrentAccount.IsVendor())
            {
                query = query.Where(jo => jo.FranchiseId == _workContext.CurrentAccount.FranchiseId);
            }

            // ************************************************
            // Analyze the search key
            // ************************************************
            bool isNum = _genericHelper.IsSearchableDigits(searchKey);
            string sKeyWord = _genericHelper.ToSearchableString(searchKey);
            // End of analysis
            // ************************************************

            if (isNum)
            {
                query = from jo in query
                        where jo.Id.ToString() == sKeyWord
                        select jo;
            }
            else
            {
                query = from jo in query
                        where jo.Company.CompanyName.Contains(sKeyWord)
                        || jo.Shift.ShiftName.Contains(sKeyWord)
                        || jo.JobOrderType.JobOrderTypeName.Contains(sKeyWord)
                        || jo.JobTitle.Contains(sKeyWord)
                        || jo.JobDescription.Contains(sKeyWord)
                        || jo.Note.Contains(sKeyWord)
                        select jo;
            }

            query = query.OrderByDescending(jo => jo.UpdatedOnUtc);

            if (maxRecordsToReturn < 1 || maxRecordsToReturn > 500)
                maxRecordsToReturn = 500;

            return query.Take(maxRecordsToReturn).ToList();
        }


        public IList<JobOrder> SearchCompanyJobOrders(Account account, string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false)
        {
            if (String.IsNullOrWhiteSpace(searchKey))
            {
                IList<JobOrder> jobOrders = new List<JobOrder>();
                return jobOrders;
            }

            var query = _jobOrderRepository.Table;

            // search within company
            query = query.Where(jo => jo.CompanyId == account.CompanyId);

            // active
            if (!showInactive)
                query = query.Where(jo => jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active);

            // deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);

            // Check account role and determine search range
            if (account.IsCompanyLocationManager())
                query = query.Where(jo => jo.CompanyLocationId == account.CompanyLocationId); // search within locatin

            if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(jo => jo.CompanyLocationId == account.CompanyLocationId && jo.CompanyDepartmentId == account.CompanyDepartmentId && // search within department
                                          jo.CompanyContactId == account.Id);

            // ************************************************
            // Analyze the search key
            // ************************************************
            bool isNum = _genericHelper.IsSearchableDigits(searchKey);
            string sKeyWord = _genericHelper.ToSearchableString(searchKey);
            // End of analysis
            // ************************************************

            if (isNum)
            {
                query = from jo in query
                        where jo.Id.ToString() == sKeyWord
                        select jo;
            }
            else
            {
                query = from jo in query
                        where jo.Company.CompanyName.Contains(sKeyWord)
                        || jo.Shift.ShiftName.Contains(sKeyWord)
                        || jo.JobOrderType.JobOrderTypeName.Contains(sKeyWord)
                        || jo.JobTitle.Contains(sKeyWord)
                        || jo.JobDescription.Contains(sKeyWord)
                        || jo.Note.Contains(sKeyWord)
                        select jo;
            }

            query = query.OrderByDescending(jo => jo.UpdatedOnUtc);

            if (maxRecordsToReturn < 1 || maxRecordsToReturn > 200)
                maxRecordsToReturn = 200;

            return query.Take(maxRecordsToReturn).ToList();
        }
        #endregion


        #region Change job order status

        public void ChangeJobOrderStatus(int jobOrderId, JobOrderStatusEnum status)
        {
            JobOrder jobOrder = this.GetJobOrderById(jobOrderId);
            if (jobOrder == null)
                return;

            if (jobOrder.JobOrderStatusId != (int)status)
            {
                jobOrder.JobOrderStatusId = (int)status;

                this.UpdateJobOrder(jobOrder);
            }
        }

        #endregion


        #region Dynamic Job Order

        public IQueryable<JobOrderOpening> GetAllJobOrderOpeningsByDate(Account account, DateTime? refDate = null)
        {
            refDate = refDate ?? DateTime.Today;

            var query = _jobOrderOpeningRepository.Table;

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.JobOrder.FranchiseId == account.FranchiseId);

            var latest = query.Where(x => x.StartDate <= refDate && (!x.EndDate.HasValue || x.EndDate.Value >= refDate))
                         .GroupBy(grp => grp.JobOrderId)
                         .Select(x => new
                         {
                             JobOrderId = x.Key,
                             StartDate = x.Max(y => y.StartDate),
                         });

            query = from q in query
                    join l in latest on new { x = q.JobOrderId, y = q.StartDate } equals new { x = l.JobOrderId, y = l.StartDate }
                    select q;

            return query;
        }

        public IQueryable<JobOrderOpening> GetAllJobOrderOpenningsByJobOrder(Guid guid)
        {
            return _jobOrderOpeningRepository.TableNoTracking.Where(x => x.JobOrder.JobOrderGuid == guid).OrderBy(x => x.Id);
        }


        public IEnumerable<JobOrderOpening> GetJobOrderOpeningsByDate(Account account, Guid guid, DateTime startDate, DateTime endDate)
        {
            var openings = Enumerable.Empty<JobOrderOpening>();
            var jobOrder = _jobOrderRepository.Table.FirstOrDefault(x => x.JobOrderGuid == guid);
            if (jobOrder != null)
                openings = jobOrder.JobOrderOpenings;

            var dates = Enumerable.Range(0, (endDate - startDate).Days + 1).Select(x => startDate.AddDays(x));
            return from d in dates
                   from o in openings.Where(o => o.StartDate <= d && (!o.EndDate.HasValue || o.EndDate >= d)).DefaultIfEmpty()
                   select new JobOrderOpening()
                   {
                       JobOrderId = jobOrder.Id,
                       JobOrder = jobOrder,
                       StartDate = d,
                       EndDate = d,
                       OpeningNumber = o != null ? o.OpeningNumber : 0,
                   };
        }


        public int GetJobOrderOpeningAvailable(int jobOrderId, DateTime inquiryDate, out JobOrderOpening[] openingChanges)
        {
            var jo = _jobOrderRepository.TableNoTracking.Where(x => x.Id == jobOrderId)
                .Include(x => x.JobOrderOpenings)
                .FirstOrDefault();

            if (jo != null)
            {
                openingChanges = jo.JobOrderOpenings
                    //.Where(y => y.StartDate <= inquiryDate)
                    .OrderBy(y => y.Id).ToArray();
                return jo.JobOrderOpenings
                    .Where(y => (!y.EndDate.HasValue || y.EndDate >= inquiryDate) && (y.StartDate <= inquiryDate))
                    .OrderByDescending(y => y.Id).Select(x => x.OpeningNumber).FirstOrDefault();
            }

            openingChanges = new JobOrderOpening[] { };
            return 0;
        }


        private JobOrderOpening CreateNewOpeningEntry(int jobOrderId, DateTime startDate, DateTime? endDate, int openings, string note)
        {
            return new JobOrderOpening()
            {
                JobOrderId = jobOrderId,
                StartDate = startDate,
                EndDate = endDate,
                OpeningNumber = openings,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Note = note,
                EnteredBy = _workContext.CurrentAccount.Id
            };
        }


        public void UpdateJobOrderOpenings(int jobOrderId, DateTime EffectiveDate, int NewOpeningNumber, string note)
        {
            var jo = _jobOrderRepository.Table.Where(x => x.Id == jobOrderId)
                .Include(x => x.JobOrderOpenings)
                .FirstOrDefault();

            if (jo != null)
            {
                DateTime? nextStartDate = null;
                JobOrderOpening newEntry = null;
                bool applied = false;

                var currentEntry = jo.JobOrderOpenings
                    .OrderByDescending(y => y.StartDate); // the list is sorted by descending order, so we always start from the last entry

                foreach (var entry in currentEntry)
                {
                    if (entry.StartDate == EffectiveDate)
                    {
                        if (!entry.EndDate.HasValue || entry.EndDate == EffectiveDate)
                        {
                            entry.OpeningNumber = NewOpeningNumber;
                            entry.EnteredBy = _workContext.CurrentAccount.Id;
                            applied = true;
                        }
                        else if (entry.EndDate > EffectiveDate)
                        {
                            newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date.AddDays(1), entry.EndDate, entry.OpeningNumber, entry.Note);
                            jo.JobOrderOpenings.Add(newEntry);

                            entry.EndDate = EffectiveDate.Date;
                            applied = true;
                        }
                    }

                    else if (entry.EndDate.HasValue && entry.EndDate.Value == EffectiveDate)
                    {
                        newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date, entry.EndDate, NewOpeningNumber, note);
                        jo.JobOrderOpenings.Add(newEntry);

                        entry.EndDate = EffectiveDate.Date.AddDays(-1);
                        applied = true;
                    }
                    
                   
                    else if (entry.StartDate < EffectiveDate)
                    {
                         if (!entry.EndDate.HasValue) 
                        {
                            entry.EndDate = EffectiveDate.Date.AddDays(-1);
                            newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date, null, NewOpeningNumber, note);
                            jo.JobOrderOpenings.Add(newEntry);
                            applied = true;
                        }
                        else  //if  entry.EndDate.HasValue 
                        {
                            if (entry.EndDate > EffectiveDate)
                            {
                                newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date.AddDays(1), entry.EndDate, entry.OpeningNumber, entry.Note);
                                jo.JobOrderOpenings.Add(newEntry);

                                newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date, EffectiveDate.Date, NewOpeningNumber, note);
                                jo.JobOrderOpenings.Add(newEntry);

                                entry.EndDate = EffectiveDate.AddDays(-1);
                                applied = true;
                            }
                            else //if (entry.EndDate < EffectiveDate)
                            {
                                if (entry.EndDate < EffectiveDate.AddDays(-1))
                                {
                                    newEntry = this.CreateNewOpeningEntry(jobOrderId, entry.EndDate.Value.AddDays(1), EffectiveDate.AddDays(-1), 0, null);
                                    jo.JobOrderOpenings.Add(newEntry);
                                }

                                newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date, null, NewOpeningNumber, note);
                                if (nextStartDate.HasValue) newEntry.EndDate = nextStartDate.Value.AddDays(-1);
                                jo.JobOrderOpenings.Add(newEntry);
                                
                                applied = true;
                            }
                           
                        }
                    }


                    else if (entry.StartDate > EffectiveDate)
                    {
                        nextStartDate = entry.StartDate;
                        continue;
                    }


                    if (applied) break;

                    nextStartDate = entry.StartDate;
                }

                if (!applied)
                {
                    newEntry = this.CreateNewOpeningEntry(jobOrderId, EffectiveDate.Date, null, NewOpeningNumber, note);
                    if (nextStartDate.HasValue) newEntry.EndDate = nextStartDate.Value.AddDays(-1);
                    jo.JobOrderOpenings.Add(newEntry);
                }

                this._jobOrderRepository.Update(jo);
            }
        }


        public void UpdateJobOrderOpeningForSelectedDate(int jobOrderId, DateTime refDate, int delta, string note)
        {
            JobOrderOpening[] _openingChanges;
            int opening = this.GetJobOrderOpeningAvailable(jobOrderId, refDate, out _openingChanges);

            if (opening + delta >= 0)
            {
                // update for the date
                this.UpdateJobOrderOpenings(jobOrderId, refDate, opening + delta, note);

                // recover for beyond
                this.UpdateJobOrderOpenings(jobOrderId, refDate.AddDays(1), opening, note);
            }
        }


        public string UpdateJobOrderOpenings(int jobOrderId, DateTime startDate, DateTime endDate, int openingNumber, string note)
        {
            var jobOrder = GetJobOrderById(jobOrderId);
            if (jobOrder == null)
                return String.Format("Job Order {0} not found", jobOrderId);

            if (endDate.Date < startDate.Date)
                endDate = startDate.Date;

            // force dates within range
            startDate = startDate >= jobOrder.StartDate ? startDate.Date : jobOrder.StartDate.Date;
            endDate = (!jobOrder.EndDate.HasValue || endDate < jobOrder.EndDate) ? endDate.Date : jobOrder.EndDate.Value.Date;

            var newEntry = new JobOrderOpening();
            var originalEntries = _jobOrderOpeningRepository.Table.Where(x => x.JobOrderId == jobOrderId)
                .Where(x => x.StartDate <= endDate && (!x.EndDate.HasValue || x.EndDate >= startDate))
                .OrderByDescending(x => x.StartDate).ToList();

            var error = "Failed to save opening changes";
            using (TransactionScope scope = new TransactionScope())
            {
                // delete any existing data that overlaps with new settings
                _jobOrderOpeningRepository.Delete(originalEntries);

                // create entries based on original settings for right & left end, which fall out side the selected date range
                foreach (var entry in originalEntries)
                {
                    if (entry.StartDate.Date < startDate)
                    {
                        newEntry = this.CreateNewOpeningEntry(jobOrderId, entry.StartDate.Date, startDate.AddDays(-1), entry.OpeningNumber, entry.Note);
                        _jobOrderOpeningRepository.Insert(newEntry);
                    }

                    if (!entry.EndDate.HasValue || entry.EndDate.Value.Date > endDate)
                    {
                        newEntry = this.CreateNewOpeningEntry(jobOrderId, endDate.AddDays(1), entry.EndDate, entry.OpeningNumber, entry.Note);
                        _jobOrderOpeningRepository.Insert(newEntry);
                    }
                }

                // create the records for new settings
                newEntry = this.CreateNewOpeningEntry(jobOrderId, startDate, endDate, openingNumber, note);
                _jobOrderOpeningRepository.Insert(newEntry);

                // commit all changes
                scope.Complete();
                error = string.Empty;
            }

            return error;
        }


        public string RemoveJobOrderOpening(int openingId)
        {
            var error = string.Empty;

            var opening = _jobOrderOpeningRepository.GetById(openingId);
            if (opening != null)
                _jobOrderOpeningRepository.Delete(opening);

            return error;
        }

        #endregion


        /// <summary>
        /// Gets all job orders with company address asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IQueryable<JobOrderWithCompanyAddress> GetAllJobOrdersWithCompanyAddressAsQueryable(Account account)
        {
            var query = _jobOrderRepository.Table;

            // query within Franchise
            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(jo => jo.FranchiseId == account.FranchiseId);

            // query within company
            query = query.Where(jo => jo.CompanyId == account.CompanyId);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager() || account.IsAdministrator()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId); // search within locatin

            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId &&
                    jo.CompanyDepartmentId > 0 &&
                    jo.CompanyDepartmentId == account.CompanyDepartmentId &&    // search within department
                    jo.CompanyContactId == account.Id);
            else if (account.IsCompanyDepartmentManager())
                query = query.Where(jo =>
                    jo.CompanyLocationId > 0 &&
                    jo.CompanyLocationId == account.CompanyLocationId &&
                    jo.CompanyDepartmentId > 0 &&
                    jo.CompanyDepartmentId == account.CompanyDepartmentId);
            else
                query = query.Where(x => false); // No role

            var result = from jo in query
                         from companyLocation in _companyLocationRepository.Table.Where(o => jo.CompanyLocationId == o.Id)
                                                                             .DefaultIfEmpty()
                         from city in _cityRepository.Table.Where(o => companyLocation.CityId == o.Id).DefaultIfEmpty()
                         from companyDepartment in _companyDepartmentRepository.Table.Where(o => jo.CompanyDepartmentId == o.Id)
                                                                               .DefaultIfEmpty()

                         orderby jo.UpdatedOnUtc descending
                         select new JobOrderWithCompanyAddress()
                         {
                             Id = jo.Id,
                             CreatedOnUtc = jo.CreatedOnUtc,
                             UpdatedOnUtc = jo.UpdatedOnUtc,
                             IsHot = jo.IsHot,
                             JobTitle = jo.JobTitle,
                             CompanyId = jo.CompanyId,
                             AddressLine1 = companyLocation.AddressLine1,
                             CompanyContactId = jo.CompanyContactId,
                             CompanyJobNumber = jo.CompanyJobNumber,
                             //Supervisor = jo.Supervisor,
                             JobOrderTypeId = jo.JobOrderTypeId,
                             JobOrderCategoryId = jo.JobOrderCategoryId,
                             ShiftId = jo.ShiftId,
                             StartDate = jo.StartDate,
                             StartTime = jo.StartTime,
                             EndDate = jo.EndDate,
                             EndTime = jo.EndTime,
                             SchedulePolicyId = jo.SchedulePolicyId,
                             BillingRateCode = jo.BillingRateCode,
                             JobOrderStatusId = jo.JobOrderStatusId,
                             IsPublished = jo.IsPublished,
                             IsInternalPosting = jo.IsInternalPosting,
                             RecruiterId = jo.RecruiterId,
                             OwnerId = jo.OwnerId,
                             Company = jo.Company,
                             CompanyLocationId = jo.CompanyLocationId,
                             CompanyDepartmentName = companyDepartment.DepartmentName,
                             CityName = city.CityName,
                             FranchiseId = jo.FranchiseId
                         };
            return result.AsQueryable();
        }


        #region Check Billing Rates

        public IList<int> DoAllJobOrdersHaveBillingRates(List<int> ids, DateTime startDate, DateTime endDate)
        {
            var result = new List<int>();

            ids.Sort();
            foreach (var id in ids)
            {
                if (!AreBillingRatesDefinedForJobOrderByDateRange(id, startDate, endDate))
                    result.Add(id);
            }

            return result;
        }
        

        public bool AreBillingRatesDefinedForJobOrderByDateRange(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var result = false;

            var rates = GetBillingRatesForJobOrderByDateRange(jobOrderId, startDate, endDate);
            if (rates.Count > 0)
            {
                var lastDeactivatedDate = rates.FirstOrDefault().EffectiveDate.AddDays(-1);
                foreach (var r in rates)
                {
                    // last one
                    if (!r.DeactivatedDate.HasValue || r.DeactivatedDate >= endDate)
                    {
                        result = true;
                        break;
                    }
                    // not continuous
                    else if (r.EffectiveDate != lastDeactivatedDate.AddDays(1))
                        break;
                    // not last one
                    else
                        lastDeactivatedDate = r.DeactivatedDate.Value;
                }
            }

            return result;
        }


        public List<CompanyBillingRate> GetBillingRatesForJobOrderByDateRange(int jobOrderId, DateTime startDate, DateTime endDate)
        {
            var jobOrder = GetJobOrderById(jobOrderId);
            if (jobOrder == null || startDate == null || endDate == null)
                return new List<CompanyBillingRate>();

            var rates = _companyBillingService.GetAllCompanyBillingRatesAsQueryable()
                        .Where(x => x.IsActive && x.CompanyId == jobOrder.CompanyId && x.CompanyLocationId == jobOrder.CompanyLocationId && x.RateCode == jobOrder.BillingRateCode)
                        .Where(x => x.EffectiveDate <= endDate && (!x.DeactivatedDate.HasValue || x.DeactivatedDate >= startDate))
                        .OrderBy(x => x.EffectiveDate);

            return rates.ToList();
        }

        #endregion


        #region Modify All job orders from one account to another account
        public void MoveAllJobOrdersToNewAccount(int companyId, int newAccountId,int prevAccountId)
        {
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("companyId", companyId);
            paras[1] = new SqlParameter("newAccountId", newAccountId);
            paras[2] = new SqlParameter("prevAccountId", prevAccountId);
            const string query = @"Update JobOrder
                                    set RecruiterId = @newAccountId
                                    where RecruiterId = @prevAccountId and CompanyId =@companyId
                                    
                                    Update JobOrder 
                                    set OwnerId = @newAccountId
                                    where OwnerId = @prevAccountId and CompanyId =@companyId
                                    ";
            int effect = _dbContext.ExecuteSqlCommand(query, false, null, paras);

        }
        #endregion
    }
}
