using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Infrastructure;
using Wfm.Services.Configuration;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Scheduling
{
    public class SchedulingDemandService : ISchedulingDemandService
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<SchedulePeriod> _schedulePeriodRepository;
        private readonly IRepository<ShiftSchedule> _shiftScheduleRepository;
        private readonly IRepository<ShiftScheduleDaily> _shiftScheduleDailyRepository;
        private readonly IRepository<CompanyShift> _companyShiftRepository;
        private readonly IRepository<CompanyJobRole> _companyJobRoleRepository;
        private readonly IRepository<EmployeeSchedule> _employeeScheduleRepository;
        private readonly IRepository<EmployeeAvailability> _employeeAvailabilityRepository;
        private readonly IRepository<CompanyOvertimeRule> _companyOvertimeRuleRepository;
        private readonly IRepository<ScheduleJobOrder> _scheduleJobOrderRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<JobOrderCategory> _jobOrderCategoryRepository;
        private readonly IRepository<JobOrderStatus> _jobOrderStatusRepository;
        private readonly IRepository<JobOrderType> _jobOrderTypeRepository;
        private readonly IRepository<EmployeeScheduleDaily> _employeeScheduleDailyRepository;
        private readonly IRepository<EmployeeScheduleDailyBreak> _employeeScheduleDailyBreakRepository;
        private readonly IRepository<CandidateJobOrder> _candidateJobOrderRepository;
        private readonly ISettingService _settingService;
        #endregion

        #region ctor
        public SchedulingDemandService(IWorkContext workContext,
            IRepository<Employee> employeeRepository,
            IRepository<SchedulePeriod> schedulePeriodRepository,
            IRepository<ShiftSchedule> shiftScheduleRepository,
            IRepository<ShiftScheduleDaily> shiftScheduleDailyRepository,
            IRepository<CompanyShift> companyShiftRepository,
            IRepository<CompanyJobRole> companyJobRoleRepository,
            IRepository<EmployeeSchedule> employeeScheduleRepository,
            IRepository<EmployeeAvailability> employeeAvailabilityRepository,
            IRepository<CompanyOvertimeRule> companyOvertimeRuleRepository,
            IRepository<ScheduleJobOrder> scheduleJobOrderRepository,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<JobOrderCategory> jobOrderCategoryRepository,
            IRepository<JobOrderStatus> jobOrderStatusRepository,
            IRepository<JobOrderType> jobOrderTypeRepository,
            IRepository<EmployeeScheduleDaily> employeeScheduleDailyRepository,
            IRepository<EmployeeScheduleDailyBreak> employeeScheduleDailyBreakRepository,
            IRepository<CandidateJobOrder> candidateJobOrderRepository,
            ISettingService settingService)
        {
            _workContext = workContext;
            _employeeRepository = employeeRepository;
            _schedulePeriodRepository = schedulePeriodRepository;
            _shiftScheduleRepository = shiftScheduleRepository;
            _shiftScheduleDailyRepository = shiftScheduleDailyRepository;
            _companyShiftRepository = companyShiftRepository;
            _companyJobRoleRepository = companyJobRoleRepository;
            _employeeScheduleRepository = employeeScheduleRepository;
            _employeeAvailabilityRepository = employeeAvailabilityRepository;
            _companyOvertimeRuleRepository = companyOvertimeRuleRepository;
            _scheduleJobOrderRepository = scheduleJobOrderRepository;
            _jobOrderRepository = jobOrderRepository;
            _jobOrderCategoryRepository = jobOrderCategoryRepository;
            _jobOrderStatusRepository = jobOrderStatusRepository;
            _jobOrderTypeRepository = jobOrderTypeRepository;
            _employeeScheduleDailyRepository = employeeScheduleDailyRepository;
            _employeeScheduleDailyBreakRepository = employeeScheduleDailyBreakRepository;
            _candidateJobOrderRepository = candidateJobOrderRepository;
            _settingService = settingService;
        }
        #endregion

        #region Schedule Period
        public IQueryable<SchedulePeriod> GetAllSchedulePeriods(int companyId)
        {
            return _schedulePeriodRepository.Table.Where(x => x.CompanyId == companyId)
                .Include(x => x.CompanyDepartment.CompanyLocation)
                .OrderByDescending(x => x.PeriodEndUtc);
        }

        public IQueryable<SchedulePeriod> GetAllSchedulePeriodsByAccount(Account account)
        {
            var result = GetAllSchedulePeriods(account.CompanyId);

            if (!account.IsCompanyAdministrator() && !account.IsCompanyHrManager())
                result = result.Where(x => x.CompanyLocationId == account.CompanyLocationId);

            return result;
        }
        public IList<int> GetCompanyJobRoleIdsBySchedulePeriod(int schedulePeriodId)
        {
            var jobRoles = GetAllSchedulePeriods(_workContext.CurrentAccount.CompanyId)
                .Where(x => x.Id == schedulePeriodId)
                .SelectMany(x => x.ShiftSchedules.SelectMany(s => s.CompanyShift.CompanyShiftJobRoles))
                .ToArray();
            return jobRoles.Select(r => r.CompanyJobRoleId).ToArray();
        }
        public decimal GetDefaultLengthInHours(int schedulePeriodId, int companyJobRoleId)
        {
            var jobRole = GetAllSchedulePeriods(_workContext.CurrentAccount.CompanyId)
                           .Where(x => x.Id == schedulePeriodId)
                           .SelectMany(x => x.ShiftSchedules.SelectMany(s => s.CompanyShift.CompanyShiftJobRoles))
                           .Where(r => r.CompanyJobRoleId == companyJobRoleId)
                           .FirstOrDefault();
            var defaultLengthInHours = 0m;
            if (jobRole != null && jobRole.CompanyShift.ShiftSchedules.FirstOrDefault() != null)
            {
                defaultLengthInHours = jobRole.CompanyShift.ShiftSchedules.FirstOrDefault().LengthInHours;
            }
            return defaultLengthInHours;
        }

        public SchedulePeriod GetSchedulePeriodById(int id)
        {
            return _schedulePeriodRepository.Table.Where(x => x.Id == id)
                .Include(x => x.CompanyDepartment.CompanyLocation)
                .Include(x => x.ShiftSchedules.Select(s => s.CompanyShift.Shift))
                .SingleOrDefault();
        }
        public void InsertCompanySchedulePeriod(SchedulePeriod entity)
        {
            entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
            _schedulePeriodRepository.Insert(entity);
        }
        public void UpdateCompanySchedulePeriod(SchedulePeriod entity)
        {
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _schedulePeriodRepository.Update(entity);
        }
        public void DeleteCompanySchedulePeriod(int id)
        {
            var entity = _schedulePeriodRepository.Table.Where(x => x.Id == id).Include(x => x.ShiftSchedules).SingleOrDefault();
            if (entity != null)
            {
                var shiftsToRemove = entity.ShiftSchedules.ToArray();
                foreach (var shift in shiftsToRemove)
                {
                    _shiftScheduleRepository.Delete(shift);
                }
                _schedulePeriodRepository.Delete(entity);
            }
        }

        public SchedulePeriod GetSchedulePeriodByCompanyAndDate(int companyId, DateTime refDate)
        {
            refDate = refDate.ToUniversalTime();
            var result = _schedulePeriodRepository.Table
                         .Include(x => x.ShiftSchedules.Select(s => s.CompanyShift.Shift))
                         .Where(x => x.CompanyId == companyId && !x.CompanyLocationId.HasValue)
                         .Where(x => x.PeriodStartUtc <= refDate && x.PeriodEndUtc >= refDate)
                         .FirstOrDefault();
            return result;
        }

        public SchedulePeriod GetSchedulePeriodByLocationAndDate(int locationId, DateTime refDate)
        {
            refDate = refDate.ToUniversalTime();
            var result = _schedulePeriodRepository.Table
                         .Include(x => x.CompanyLocation)
                         .Include(x => x.ShiftSchedules.Select(s => s.CompanyShift.Shift))
                         .Where(x => x.CompanyLocationId == locationId)
                         .Where(x => x.PeriodStartUtc <= refDate && x.PeriodEndUtc >= refDate)
                         .FirstOrDefault();

            return result;
        }


        public IQueryable<SchedulePeriod> GetPreviousSchedulePeriodsByLocationAndDate(int locationId, DateTime refDate, bool validOnly = true)
        {
            refDate = refDate.AddDays(DayOfWeek.Sunday - refDate.DayOfWeek);
            var refDateUtc = refDate.ToUniversalTime();
            var periods = _schedulePeriodRepository.Table.Where(x => x.CompanyLocationId == locationId && x.PeriodEndUtc < refDateUtc);

            // valid: with shift schedule data
            if (validOnly)
            {
                var validIds = _shiftScheduleRepository.Table.Include(x => x.CompanyShift)
                               .Where(x => x.CompanyShift.ExpiryDate.HasValue && x.CompanyShift.ExpiryDate < refDate)
                               .Select(x => x.SchedulePeriodId).Distinct();
                periods = periods.Where(x => validIds.Contains(x.Id));
            }

            return periods.OrderByDescending(x => x.PeriodStartUtc);
        }


        public IQueryable<SchedulePeriod> GetLaterSchedulePeriodsByLocationAndDate(int locationId, DateTime refDate, bool validOnly = true)
        {
            refDate = refDate.AddDays(DayOfWeek.Saturday - refDate.DayOfWeek);
            var refDateUtc = refDate.ToUniversalTime();
            var periods = _schedulePeriodRepository.Table.Where(x => x.CompanyLocationId == locationId && x.PeriodEndUtc > refDateUtc);

            // valid: with shift schedule data
            if (validOnly)
            {
                var validIds = _shiftScheduleRepository.Table.Include(x => x.CompanyShift)
                               .Where(x => x.CompanyShift.ExpiryDate.HasValue && x.CompanyShift.ExpiryDate < refDate)
                               .Select(x => x.SchedulePeriodId).Distinct();
                periods = periods.Where(x => validIds.Contains(x.Id));
            }

            return periods.OrderByDescending(x => x.PeriodStartUtc);
        }


        public SchedulePeriod GetDemandByDemandDetails(int schedulePeriodId, int departmentId, int jobRoleId, TimeSpan startTime, decimal length)
        {
            //TODO
            return null;
        }

        #endregion

        #region Shift Schedule
        public IEnumerable<ShiftSchedule> GetShiftsOfSchedulePeriod(int schedulePeriodId)
        {
            var shiftIds = _shiftScheduleRepository.Table.Where(x => x.SchedulePeriodId == schedulePeriodId).Select(x => x.CompanyShiftId).ToArray();
            var schedulePeriod = _schedulePeriodRepository.Table.Where(x => x.Id == schedulePeriodId).Include(x => x.CompanyDepartment).Single();
            var companyLevelSchedule = schedulePeriod.CompanyDepartment == null;
            var locationId = !companyLevelSchedule ? schedulePeriod.CompanyDepartment.CompanyLocationId : 0;
            var existingShfits = _shiftScheduleRepository.Table.Where(x => x.SchedulePeriodId == schedulePeriodId)
                .Include(x => x.CompanyShift.Shift)
                .Include(x => x.CompanyShift.CompanyShiftJobRoles)
                .Include(x => x.SchedulePeriod).ToArray();
            var unplannedShifts = _companyShiftRepository.Table
                    .Where(x => x.CompanyId == schedulePeriod.CompanyId)
                    .Where(x => (x.CompanyDepartment.CompanyLocation == null && companyLevelSchedule) ||
                        (x.CompanyDepartment.CompanyLocationId == locationId && x.CompanyDepartmentId == schedulePeriod.CompanyDepartmentId))
                    .Where(x => !shiftIds.Contains(x.Id))
                    .Include(x => x.Shift)
                    .Include(x => x.CompanyShiftJobRoles)
                    .ToArray();
            var unplannedShiftPeriods = unplannedShifts
                    .Select(x => new ShiftSchedule
                    {
                        SchedulePeriod = schedulePeriod,
                        SchedulePeriodId = schedulePeriodId,
                        CompanyShift = x,
                        CompanyShiftId = x.Id,
                    });
            return existingShfits.Union(unplannedShiftPeriods);
        }
        public void UpdateShiftsOfSchedulePeriod(int schedulePeriodId, IEnumerable<ShiftSchedule> shifts)
        {
            var schedulePeriod = _schedulePeriodRepository.Table.Where(x => x.Id == schedulePeriodId)
                .Include(x => x.ShiftSchedules).Single();
            foreach (var shift in shifts)
            {
                var toUpdate = schedulePeriod.ShiftSchedules.Where(x => x.CompanyShiftId == shift.CompanyShiftId).FirstOrDefault();
                if (toUpdate != null && toUpdate.Id != 0)
                {
                    toUpdate.UpdatedOnUtc = DateTime.UtcNow;
                    toUpdate.CompanyShiftId = shift.CompanyShiftId;
                    toUpdate.SundaySwitch = shift.SundaySwitch;
                    toUpdate.MondaySwitch = shift.MondaySwitch;
                    toUpdate.TuesdaySwitch = shift.TuesdaySwitch;
                    toUpdate.WednesdaySwitch = shift.WednesdaySwitch;
                    toUpdate.ThursdaySwitch = shift.ThursdaySwitch;
                    toUpdate.FridaySwitch = shift.FridaySwitch;
                    toUpdate.SaturdaySwitch = shift.SaturdaySwitch;
                    toUpdate.StartTimeOfDayTicks = shift.StartTimeOfDayTicks;
                    toUpdate.LengthInHours = shift.LengthInHours;
                    SyncShiftScheduleDaily(toUpdate);
                }
                else
                {
                    shift.UpdatedOnUtc = shift.CreatedOnUtc = DateTime.UtcNow;
                    shift.CompanyShift = null;
                    schedulePeriod.ShiftSchedules.Add(shift);
                    SyncShiftScheduleDaily(shift);
                }
            }
            _schedulePeriodRepository.Update(schedulePeriod);
        }

        private void SyncShiftScheduleDaily(ShiftSchedule shiftSchedule)
        {
            var toDelete = _shiftScheduleDailyRepository.Table.Where(x => x.CompanyShiftId == shiftSchedule.CompanyShiftId && x.SchedulePeriodId == shiftSchedule.SchedulePeriodId).ToArray();
            foreach (var d in toDelete)
            {
                _shiftScheduleDailyRepository.Delete(d);
            }
            //
            var schedulePeriod = _schedulePeriodRepository.Table.Where(x => x.Id == shiftSchedule.SchedulePeriodId).Single();
            for (var date = schedulePeriod.PeriodStartUtc.ToLocalTime().Date; date <= schedulePeriod.PeriodEndUtc.ToLocalTime().Date; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;
                if (shiftSchedule.IsShiftScheduled(dayOfWeek))
                {
                    _shiftScheduleDailyRepository.Insert(new ShiftScheduleDaily
                    {
                        CompanyShiftId = shiftSchedule.CompanyShiftId,
                        SchedulePeriodId = shiftSchedule.SchedulePeriodId,
                        ScheduelDate = date,
                        LengthInHours = shiftSchedule.LengthInHours,
                        StartTimeOfDayTicks = shiftSchedule.StartTimeOfDayTicks,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    });
                }
            }
            SyncJobOrders(shiftSchedule);
        }

        public void SyncJobOrders(ShiftSchedule shiftSchedule)
        {
            var jobTitles = shiftSchedule.CompanyShift.CompanyShiftJobRoles.Select(r => Tuple.Create(r.CompanyJobRoleId, r.CompanyJobRole.Name.ToUpper(),
                r.CompanyJobRole.Description, r.Supervisor.FullName)).ToArray();
            var startDate = shiftSchedule.SchedulePeriod.PeriodStartUtc.ToLocalTime();
            var endDate = shiftSchedule.SchedulePeriod.PeriodEndUtc.ToLocalTime();
            var startTime = shiftSchedule.StartTimeOfDay;
            var endTime = shiftSchedule.EndTimeOfDay;
            var jobOrderCategoryId = _jobOrderCategoryRepository.Table.Where(x => x.CategoryName == "Default").Select(x => x.Id).FirstOrDefault();
            var jobOrderStatusId = _jobOrderStatusRepository.Table.Where(x => x.JobOrderStatusName == "Active").Select(x => x.Id).FirstOrDefault();
            var _jobOrderTypeId = _jobOrderTypeRepository.Table.Where(x => x.JobOrderTypeName == "On-going").Select(x => x.Id).FirstOrDefault();
            foreach (var jTitle in jobTitles)
            {
                var jobOrder = FindJobOrder(shiftSchedule.SchedulePeriodId, shiftSchedule.CompanyShiftId, jTitle.Item1);
                if (jobOrder != null)
                {
                    jobOrder.JobTitle = jTitle.Item2;
                    jobOrder.StartDate = startDate;
                    jobOrder.StartTime = startDate.Add(startTime);
                    jobOrder.EndDate = endDate;
                    jobOrder.EndTime = endDate.Add(endTime);
                    jobOrder.JobDescription = jTitle.Item3;
                    jobOrder.UpdatedOnUtc = DateTime.UtcNow;
                    //jobOrder.Supervisor = jTitle.Item4;
                    _jobOrderRepository.Update(jobOrder);
                }
                else
                {
                    jobOrder = new JobOrder
                    {
                        CompanyId = shiftSchedule.CompanyShift.CompanyId,
                        ShiftId = shiftSchedule.CompanyShift.ShiftId,
                        JobTitle = jTitle.Item2,
                        StartDate = startDate,
                        StartTime = startDate.Add(startTime),
                        EndDate = endDate,
                        EndTime = endDate.Add(endTime),
                        FranchiseId = _workContext.CurrentFranchise.Id,
                        UpdatedOnUtc = DateTime.UtcNow,
                        CreatedOnUtc = DateTime.UtcNow,
                        BillingRateCode = "GL / D/N / 1", // hardcode for now
                        JobDescription = jTitle.Item3,
                        JobOrderCategoryId = jobOrderCategoryId,
                        JobOrderStatusId = jobOrderStatusId,
                        JobOrderTypeId = _jobOrderTypeId,
                        //Supervisor = jTitle.Item4,
                    };
                    _jobOrderRepository.Insert(jobOrder);
                    _scheduleJobOrderRepository.Insert(new ScheduleJobOrder
                    {
                        SchedulePeriodId = shiftSchedule.SchedulePeriodId,
                        CompanyShiftId = shiftSchedule.CompanyShiftId,
                        JobRoleId = jTitle.Item1,
                        JobOrder = jobOrder,
                        UpdatedOnUtc = DateTime.UtcNow,
                        CreatedOnUtc = DateTime.UtcNow,
                    });
                }
            }
        }

        public void CopySchedulePeriodAndShifts(int schedulePeriodId, DateTime toStartDate, DateTime toEndDate)
        {
            var schedulePeriod = _schedulePeriodRepository.Table.Where(x => x.Id == schedulePeriodId).Include(x => x.ShiftSchedules).Single();
            var utcNow = DateTime.UtcNow;
            if (schedulePeriod != null)
            {
                var newSchedulePeriod = new SchedulePeriod
                {
                    ShiftScheduleDays = new List<ShiftScheduleDaily>(),
                    PeriodStartUtc = toStartDate.ToUniversalTime(),
                    PeriodEndUtc = toEndDate.ToUniversalTime(),
                    CompanyId = schedulePeriod.CompanyId,
                    CompanyLocationId = schedulePeriod.CompanyLocationId,
                    CompanyDepartmentId = schedulePeriod.CompanyDepartmentId,
                    //
                    CreatedOnUtc = utcNow,
                    UpdatedOnUtc = utcNow,
                    //
                    ShiftSchedules = schedulePeriod.ShiftSchedules.Select(x => new ShiftSchedule
                    {
                        CompanyShiftId = x.CompanyShiftId,
                        LengthInHours = x.LengthInHours,
                        StartTimeOfDayTicks = x.StartTimeOfDayTicks,
                        //
                        SundaySwitch = x.SundaySwitch,
                        MondaySwitch = x.MondaySwitch,
                        TuesdaySwitch = x.TuesdaySwitch,
                        WednesdaySwitch = x.WednesdaySwitch,
                        ThursdaySwitch = x.ThursdaySwitch,
                        FridaySwitch = x.FridaySwitch,
                        SaturdaySwitch = x.SaturdaySwitch,
                        //
                        AlternativeSundaySwitch = x.AlternativeSundaySwitch,
                        AlternativeMondaySwitch = x.AlternativeMondaySwitch,
                        AlternativeTuesdaySwitch = x.AlternativeTuesdaySwitch,
                        AlternativeWednesdaySwitch = x.AlternativeWednesdaySwitch,
                        AlternativeThursdaySwitch = x.AlternativeThursdaySwitch,
                        AlternativeFridaySwitch = x.AlternativeFridaySwitch,
                        AlternativeSaturdaySwitch = x.AlternativeSaturdaySwitch,
                        //
                        CreatedOnUtc = utcNow,
                        UpdatedOnUtc = utcNow,
                    }).ToArray(),
                };
                var scheduleDate = toStartDate;
                for (int day = 0; scheduleDate <= toEndDate; day++)
                {
                    foreach (var shift in newSchedulePeriod.ShiftSchedules)
                    {
                        if (shift.IsShiftScheduled(toStartDate.AddDays(day).DayOfWeek))
                        {
                            newSchedulePeriod.ShiftScheduleDays.Add(new ShiftScheduleDaily
                            {
                                CompanyShiftId = shift.CompanyShiftId,
                                ScheduelDate = scheduleDate,
                                StartTimeOfDayTicks = shift.StartTimeOfDayTicks,
                                LengthInHours = shift.LengthInHours,
                                //
                                CreatedOnUtc = utcNow,
                                UpdatedOnUtc = utcNow,
                            });
                        }
                    }
                    scheduleDate = scheduleDate.AddDays(1);
                }
                _schedulePeriodRepository.Insert(newSchedulePeriod);
            }
        }

        public JobOrder FindJobOrder(int schedulePeriodId, int companyShiftId, int companyJobRoleId)
        {
            return _scheduleJobOrderRepository.Table
                .Where(x => x.SchedulePeriodId == schedulePeriodId &&
                x.CompanyShiftId == companyShiftId &&
                x.JobRoleId == companyJobRoleId)
                .Select(x => x.JobOrder).FirstOrDefault();
        }
        #endregion

        #region Daily Shift Schedule
        public void InsertDailyShiftSchedule(ShiftScheduleDaily entity)
        {
            entity.UpdatedOnUtc = entity.CreatedOnUtc = DateTime.UtcNow;
            _shiftScheduleDailyRepository.Insert(entity);
        }

        public void UpdateDailyShiftSchedule(ShiftScheduleDaily entity)
        {
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _shiftScheduleDailyRepository.Update(entity);
        }

        public void DeleteDailyShiftSchedule(ShiftScheduleDaily entity)
        {
            _shiftScheduleDailyRepository.Delete(entity);
        }
        public IQueryable<ShiftScheduleDaily> GetDailyShiftSchedule(int schedulePeriodId)
        {
            var result = _shiftScheduleDailyRepository.TableNoTracking.Where(x => x.SchedulePeriodId == schedulePeriodId);
            return result;
        }
        #endregion

        #region Employee View
        public IEnumerable<EmployeeSchedule> GetEmployeeScheduleOfPeriod(int schedulePeriodId)
        {
            var employees = FilterEmployees(_employeeRepository.Table.Where(x => !x.IsDeleted && x.IsActive));
            var scheduledEmployeeIds = _employeeScheduleRepository.Table.Where(x => x.SchedulePeriodId == schedulePeriodId)
                .Select(x => x.EmployeeId).ToArray();
            var schedulePeriod = _schedulePeriodRepository.GetById(schedulePeriodId);
            var scheduledEmployees = _employeeScheduleRepository.Table
                .Join(employees, s => s.EmployeeId, e => e.Id, (s, e) => s)
                .Where(x => x.SchedulePeriodId == schedulePeriodId)
                .Include(x => x.Employee.CompanyLocation)
                .Include(x => x.Employee.EmployeeJobRoles)
                .Include(x => x.Employee.EmployeeJobRoles.Select(y => y.CompanyJobRole))
                .Include(x => x.CompanyShift.Shift)
                .Include(x => x.JobRole)
                .Include(x => x.SchedulePeriod)
                .OrderByDescending(x => x.UpdatedOnUtc)
                .ToArray();
            var unscheduledEmployees = employees
                .Where(x => !scheduledEmployeeIds.Contains(x.Id))
                .Where(x => x.IsActive && !x.IsDeleted)
                .Include(x => x.CompanyLocation)
                .Include(x => x.EmployeeJobRoles)
                .Include(x => x.EmployeeJobRoles.Select(y => y.CompanyJobRole))
                .OrderByDescending(x => x.UpdatedOnUtc)
                .ToArray()
                .Select(x => new EmployeeSchedule
                {
                    Employee = x,
                    EmployeeId = x.Id,
                    SchedulePeriod = schedulePeriod,
                    SchedulePeriodId = schedulePeriodId,
                    CompanyShift = null,
                }).ToArray();
            return scheduledEmployees.Union(unscheduledEmployees);
        }

        public IEnumerable<IEmployeeScheduleDetailModel> PreviewEmployeeScheduleDetail(
            int schedulePeriodId, int employeeId, int companyShiftId, int jobRoleId)
        {
            var schedulePeriod = _schedulePeriodRepository.Table
                .Where(x => x.Id == schedulePeriodId)
                .Include(x => x.ShiftScheduleDays.Select(y => y.CompanyShift.Shift))
                .Include(x => x.ShiftScheduleDays.Select(y => y.CompanyShift.CompanyShiftJobRoles.Select(r => r.CompanyJobRole)))
                .FirstOrDefault();
            var employee = _employeeRepository.Table
                .Where(x => x.Id == employeeId)
                .Where(x => x.IsActive && !x.IsDeleted)
                .Include(x => x.EmployeeJobRoles)
                .Include(x => x.EmployeeJobRoles.Select(y => y.CompanyJobRole))
                .FirstOrDefault();
            if (schedulePeriod != null && employee != null)
            {
                //var shiftSchedule = schedulePeriod.ShiftSchedules
                //    .Where(y => y.CompanyShiftId == companyShiftId)
                //    .FirstOrDefault();
                var shiftScheduleDays = schedulePeriod.ShiftScheduleDays
                    .Where(y => y.CompanyShiftId == companyShiftId);
                for (var date = schedulePeriod.PeriodStartUtc.ToLocalTime().Date; date <= schedulePeriod.PeriodEndUtc.ToLocalTime().Date; date = date.AddDays(1))
                {
                    var shiftScheduleDay = shiftScheduleDays.Where(d => d.ScheduelDate == date).FirstOrDefault();
                    if (shiftScheduleDay != null)
                    {
                        //var jobRole = DeterminJobRole(employee.EmployeeJobRoles, shiftSchedule.CompanyShift.CompanyShiftJobRoles);
                        var jobRoles = shiftScheduleDay.CompanyShift.CompanyShiftJobRoles.Where(x => x.CompanyJobRoleId == jobRoleId);
                        foreach (var jobRole in jobRoles)
                        {
                            var startTicks = shiftScheduleDay.StartTimeOfDayTicks;
                            var endTicks = shiftScheduleDay.StartTimeOfDayTicks + (long)(TimeSpan.TicksPerHour * shiftScheduleDay.LengthInHours);
                            var result = EngineContext.Current.Resolve<IEmployeeScheduleDetailModel>();
                            result.ScheduleDate = date;
                            result.StartTimeTicks = startTicks;
                            result.EndTimeTicks = endTicks;
                            result.CompanyJobRoleId = jobRole.CompanyJobRoleId;
                            result.EntryTitle = string.Format("{0} ({1}/{2})", jobRole.CompanyJobRole.Name, jobRole.MandantoryRequiredCount, jobRole.ContingencyRequiredCount);
                            result.ErrorWarnings = GetScheduleErrorAndWarning(schedulePeriod.Id, date, startTicks, endTicks, jobRole.CompanyJobRoleId, employeeId,
                                shiftScheduleDay.CompanyShiftId, 0).ToArray();
                            yield return result;
                        }
                    }
                }
            }
        }

        private IEnumerable<IScheduleDetailErrorModel> GetScheduleErrorAndWarning(int schedulePeriodId, DateTime scheduleDate, long startTicks, long endTicks,
            int jobRoleId, int employeeId, int companyShiftId, int employeeScheduleId)
        {
            #region Data Preparation
            IScheduleDetailErrorModel result;
            //
            var emp = _employeeRepository.GetById(employeeId);
            var shift = _shiftScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId)
                .Select(x => x.CompanyShift.Shift).FirstOrDefault();
            // company overtime Policy
            var overTimePolicies = _companyOvertimeRuleRepository.Table
                .Include(x => x.OvertimeRuleSetting)
                .Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId && x.IsActive).ToArray();
            if (!overTimePolicies.Any()) throw new InvalidOperationException("Company Overtime Policy hasn't been configured");
            var defaultOvertimePerEmployeePerWeek = overTimePolicies.Min(x => x.OvertimeRuleSetting.ApplyAfter);
            // get other settings
            var maxHoursPerEmployeePerWeek = _settingService.GetSettingByKey<int>("Scheduling.MaxHoursPerEmployeePerWeek");
            var backToBackMinGapCheckShiftHoursTotalLowerLimit = _settingService.GetSettingByKey<int>("Scheduling.BackToBackMinGapCheckShiftHoursTotalLowerLimit");
            var backToBackScheduleGapMinimal = _settingService.GetSettingByKey<int>("CandidatePlacement.MinHoursBetweenTwoShifts");
            //
            var startSpan = new TimeSpan(startTicks);
            var endSpan = new TimeSpan(endTicks);
            var utcDate = scheduleDate.ToUniversalTime();
            var startOfWeek = scheduleDate.StartOfWeek(DayOfWeek.Sunday);
            var endOfWeek = startOfWeek.AddDays(7);
            #endregion

            // rule #1.	Employee Schedule Preference - Unavailable – Error
            var availabilities = _employeeAvailabilityRepository.Table.Where(x => x.EmployeeId == employeeId && x.StartDate <= scheduleDate && x.EndDate >= scheduleDate).ToArray();
            if (availabilities.Any(x => x.EmployeeAvailabilityType == EmployeeAvailabilityType.Unavailable &&
                 ((x.StartTimeOfDay <= startSpan && x.EndTimeOfDay >= startSpan) || (x.StartTimeOfDay <= endSpan && x.EndTimeOfDay >= endSpan))))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.ErrorMessage = string.Format("There are unavailable hours of {1} in {0:D} for scheduled shift", scheduleDate, emp);
                yield return result;
            }
            // rule #2.	Employee Schedule Preference – Not Available – Warning
            if (availabilities.Any(x => x.EmployeeAvailabilityType == EmployeeAvailabilityType.Available && x.StartTimeOfDay <= startSpan && x.EndTimeOfDay >= endSpan))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Major;
                result.ErrorMessage = string.Format("Scheduled shift in {0:D} is not explicitly marked as available in employee {1} preference.", scheduleDate, emp);
                yield return result;
            }
            // rule #3.	Employee Schedule Conflicts – Error
            var anotherScheduels = _employeeScheduleRepository.Table.Where(x => x.EmployeeId == employeeId && x.Id != employeeScheduleId &&
                x.SchedulePeriod.PeriodStartUtc <= utcDate && x.SchedulePeriod.PeriodEndUtc >= utcDate)
                .Where(x => !x.ForDailyAdhoc)
                .Include(x => x.SchedulePeriod.ShiftScheduleDays.Select(y => y.CompanyShift))
                .ToArray();
            if (anotherScheduels.Any(x => x.SchedulePeriod.ShiftScheduleDays
                .Where(d => d.ScheduelDate == scheduleDate && ((d.StartTimeOfDay <= startSpan && d.EndTimeOfDay >= startSpan) || (d.StartTimeOfDay <= endSpan && d.EndTimeOfDay >= endSpan)))
                .Select(y => y.CompanyShiftId).Contains(x.CompanyShiftId)))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.ErrorMessage = string.Format("Scheduled shift in {0:D} has conflicts with other scheduled shift for {1}.", scheduleDate, emp);
                yield return result;
            }
            var anotherAdhoc = _employeeScheduleDailyRepository.Table
                .Where(x => x.ReplacementEmployeeId == employeeId || (!x.ReplacementEmployeeId.HasValue && x.EmployeeSchedule.EmployeeId == employeeId))
                .Where(x => !x.IsDeleted)
                .Where(x => x.ScheduelDate == scheduleDate)
                .Where(x => x.EmployeeScheduleId != employeeScheduleId).ToArray();
            if (anotherAdhoc.Any(x => (x.StartTimeOfDay <= startSpan && x.EndTimeOfDay >= startSpan) || (x.StartTimeOfDay <= endSpan && x.EndTimeOfDay >= endSpan)))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.ErrorMessage = string.Format("Scheduled in {0:D} has conflicts with other scheduled for {1}.", scheduleDate, emp);
                yield return result;
            }

            // rule #4.	Employee Overscheduled (More than 60 hours any week) – Error
            var totalHours = anotherScheduels.SelectMany(x => x.SchedulePeriod.ShiftScheduleDays)
                .Where(d => d.ScheduelDate >= startOfWeek && d.ScheduelDate <= endOfWeek)
                .Sum(d => d.LengthInHours) + ((endTicks - startTicks) / TimeSpan.TicksPerHour);
            if (totalHours > maxHoursPerEmployeePerWeek)
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.ErrorMessage = string.Format("Scheduled week of {2} at {0:D} exceeds the limitation of hours {1}.", scheduleDate, maxHoursPerEmployeePerWeek, emp);
                yield return result;
            }
            // rule #5.	Employee Overscheduled (More than 40 hours any week) - Warning
            if (totalHours > defaultOvertimePerEmployeePerWeek)
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Major;
                result.ErrorMessage = string.Format("Scheduled week of {2} at {0:D} exceeds the overtime limitaion of hours {1}.", scheduleDate, defaultOvertimePerEmployeePerWeek, emp);
                yield return result;
            }
            // rule #6.	Employee Job Title not match Job Role – Warning
            var _employeeWithRoles = _employeeRepository.Table
                .Include(e => e.EmployeeJobRoles)
                .Where(x => x.Id == employeeId).First();
            if (!_employeeWithRoles.EmployeeJobRoles.Any(r => r.CompanyJobRoleId == jobRoleId
                && (!r.EffectiveDate.HasValue || r.EffectiveDate <= scheduleDate)
                && (!r.ExpiryDate.HasValue || r.ExpiryDate >= scheduleDate)))
            {
                var jobRole = _companyJobRoleRepository.Table.Where(x => x.Id == jobRoleId).Include(x => x.Position).FirstOrDefault();
                //
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.EmployeeId = employeeId;
                result.Scope = ScheduleWarningScope.Employee;
                result.LevelOfError = ScheduleWarningLevel.Major;
                result.ErrorMessage = string.Format("Job role {0}/{1} is not effective role of employee {2}.", jobRole.Name, jobRole.Position.Name, emp);
                yield return result;
            }
            // rule #7.	More employees scheduled than required and contingency – Error
            var vacancy = GetShiftVacancyInPeriod(companyShiftId, schedulePeriodId, jobRoleId, employeeId);
            var vacancyRequired = GetShiftVacancyInPeriod(companyShiftId, schedulePeriodId, jobRoleId, employeeId, true);
            if (vacancy < 1)
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.Scope = ScheduleWarningScope.Shift;
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.ErrorMessage = string.Format("More employees will be scheduled than required and contingency of shift {0}.", shift);
                yield return result;
            }
            // rule #8.	More employees scheduled than required – Warning
            if (vacancyRequired < 1)
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.Scope = ScheduleWarningScope.Shift;
                result.LevelOfError = ScheduleWarningLevel.Major;
                result.ErrorMessage = string.Format("More employees will be scheduled than required of shift {0}", shift);
                yield return result;
            }
            // rule #9.	Gap between two back to back shifts (total work is more than 13 hours) of same employee is less than 8 hours – Error 
            var previousShift = anotherScheduels.SelectMany(x => x.SchedulePeriod.ShiftScheduleDays)
                .Where(d => d.ScheduelDate <= scheduleDate && d.StartTimeOfDayTicks < startTicks)
                .OrderByDescending(d => d.ScheduelDate).FirstOrDefault();
            var nextShift = anotherScheduels.SelectMany(x => x.SchedulePeriod.ShiftScheduleDays)
                .Where(d => d.ScheduelDate >= scheduleDate && d.StartTimeOfDayTicks > startTicks)
                .OrderBy(d => d.ScheduelDate).FirstOrDefault();
            if (previousShift != null && ((previousShift.LengthInHours + (decimal)(TimeSpan.FromTicks(endTicks - startTicks).TotalHours))
                    > backToBackMinGapCheckShiftHoursTotalLowerLimit) &&
                (scheduleDate.AddTicks(startTicks) - previousShift.ScheduelDate.Add(previousShift.EndTimeOfDay)
                < new TimeSpan(backToBackScheduleGapMinimal * TimeSpan.TicksPerHour)))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.Scope = ScheduleWarningScope.Employee;
                result.ErrorMessage = string.Format("Gap between previous shift to this shift (total work is more than {0} hours) of same employee {2} is less than {1} hours.",
                    backToBackMinGapCheckShiftHoursTotalLowerLimit, backToBackScheduleGapMinimal, emp);
                yield return result;
            }
            if (nextShift != null && ((nextShift.LengthInHours + (decimal)(TimeSpan.FromTicks(endTicks - startTicks).TotalHours))
                    > backToBackMinGapCheckShiftHoursTotalLowerLimit) &&
                (nextShift.ScheduelDate.Add(nextShift.StartTimeOfDay) - scheduleDate.AddTicks(endTicks)
                < new TimeSpan(backToBackScheduleGapMinimal * TimeSpan.TicksPerHour)))
            {
                result = EngineContext.Current.Resolve<IScheduleDetailErrorModel>();
                result.LevelOfError = ScheduleWarningLevel.Error;
                result.Scope = ScheduleWarningScope.Employee;
                result.ErrorMessage = string.Format("Gap between this shift to next shift (total work is more than {0} hours) of same employee {2} is less than {1} hours.",
                    backToBackMinGapCheckShiftHoursTotalLowerLimit, backToBackScheduleGapMinimal, emp);
                yield return result;
            }
        }

        private IEnumerable<IScheduleDetailErrorModel> ValidateEmployeeSchedule(EmployeeSchedule schedule)
        {
            var schedulePeriod = _schedulePeriodRepository.Table
                .Where(x => x.Id == schedule.SchedulePeriodId)
                .Include(x => x.ShiftScheduleDays.Select(y => y.CompanyShift.Shift))
                .Include(x => x.ShiftScheduleDays.Select(y => y.CompanyShift.CompanyShiftJobRoles.Select(r => r.CompanyJobRole)))
                .FirstOrDefault();
            var employee = _employeeRepository.Table
                .Where(x => x.Id == schedule.EmployeeId)
                .Where(x => x.IsActive && !x.IsDeleted)
                .Include(x => x.EmployeeJobRoles)
                .Include(x => x.EmployeeJobRoles.Select(y => y.CompanyJobRole))
                .FirstOrDefault();
            var companyShiftId = schedule.CompanyShiftId;
            if (schedulePeriod != null && employee != null)
            {
                var shiftScheduleDays = schedulePeriod.ShiftScheduleDays
                    .Where(y => y.CompanyShiftId == companyShiftId);
                for (var date = schedulePeriod.PeriodStartUtc.ToLocalTime().Date; date <= schedulePeriod.PeriodEndUtc.ToLocalTime().Date; date = date.AddDays(1))
                {
                    var dayOfWeek = date.DayOfWeek;
                    var shiftScheduleDay = shiftScheduleDays.Where(d => d.ScheduelDate == date).FirstOrDefault();
                    if (shiftScheduleDay != null)
                    {
                        //var jobRole = DeterminJobRole(employee.EmployeeJobRoles, shiftSchedule.CompanyShift.CompanyShiftJobRoles);
                        var jobRoles = shiftScheduleDay.CompanyShift.CompanyShiftJobRoles.Where(x => x.CompanyJobRoleId == schedule.JobRoleId);
                        foreach (var jobRole in jobRoles)
                        {
                            var startTicks = shiftScheduleDay.StartTimeOfDayTicks;
                            var endTicks = shiftScheduleDay.StartTimeOfDayTicks + (long)(TimeSpan.TicksPerHour * shiftScheduleDay.LengthInHours);
                            var result = GetScheduleErrorAndWarning(schedulePeriod.Id, date, startTicks, endTicks, jobRole.CompanyJobRoleId, schedule.EmployeeId,
                                shiftScheduleDay.CompanyShiftId, schedule.Id).ToArray();
                            foreach (var entry in result)
                            {
                                yield return entry;
                            }
                        }
                    }
                }
            }
        }
        private void MarkTimeOffForEmployeeSchedule(EmployeeSchedule schedule)
        {
            var periodStartLocal = schedule.SchedulePeriod.PeriodStartUtc.ToLocalTime();
            var periodEndLocal = schedule.SchedulePeriod.PeriodEndUtc.ToLocalTime();
            var timeOffBookings = _employeeRepository.TableNoTracking
                .Where(x => x.Id == schedule.EmployeeId)
                .SelectMany(x => x.EmployeeTimeoffBookings)
                .Where(b => (b.TimeOffStartDateTime >= periodStartLocal && b.TimeOffStartDateTime <= periodEndLocal) ||
                    (b.TimeOffEndDateTime >= periodStartLocal && b.TimeOffEndDateTime <= periodEndLocal) &&
                    (!b.IsRejected.HasValue || !b.IsRejected.Value)).ToArray();
            foreach (var booking in timeOffBookings)
            {
                int delta = 0;
                while (booking.TimeOffStartDateTime.Date.AddDays(delta) < booking.TimeOffEndDateTime.Date)
                {
                    var scheduleDate = booking.TimeOffStartDateTime.Date.AddDays(delta);
                    var daily = _employeeScheduleDailyRepository.Table
                        .Where(x => x.EmployeeScheduleId == schedule.Id)
                        .Where(x => x.ScheduelDate == scheduleDate).FirstOrDefault();
                    if (daily != null)
                    {
                        // TO DO: handle half day timeoff
                        //if (delta == 0 && booking.TimeOffStartDateTime.TimeOfDay.Hours < 12)
                        daily.IsDeleted = true;
                        _employeeScheduleDailyRepository.Update(daily);
                    }
                    else
                    {
                        daily = new EmployeeScheduleDaily
                        {
                            EmployeeScheduleId = schedule.Id,
                            ScheduelDate = scheduleDate,
                            IsDeleted = true,
                        };
                        _employeeScheduleDailyRepository.Insert(daily);
                    }
                    //
                    delta++;
                }
            }
        }
        public void InsertEmployeeSchedule(EmployeeSchedule schedule, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings)
        {
            errorsAndWarnings = ValidateEmployeeSchedule(schedule).ToArray();
            if (!errorsAndWarnings.Any(x => x.LevelOfError == ScheduleWarningLevel.Error))
            {
                schedule.CreatedOnUtc = schedule.UpdatedOnUtc = DateTime.UtcNow;
                _employeeScheduleRepository.Insert(schedule);
                MarkTimeOffForEmployeeSchedule(schedule);
            }
        }
        public void UpdateEmployeeSchedule(EmployeeSchedule schedule, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings)
        {
            errorsAndWarnings = ValidateEmployeeSchedule(schedule).ToArray();
            if (!errorsAndWarnings.Any(x => x.LevelOfError == ScheduleWarningLevel.Error))
            {
                schedule.UpdatedOnUtc = DateTime.UtcNow;
                _employeeScheduleRepository.Update(schedule);
                MarkTimeOffForEmployeeSchedule(schedule);
            }
        }
        public void DeleteEmployeeSchedule(int employeeScheduleId)
        {
            var entry = _employeeScheduleRepository.Table.Where(x => x.Id == employeeScheduleId)
                .Include(x => x.EmployeeScheduleDays.Select(y => y.Breaks)).FirstOrDefault();
            if (entry != null)
            {
                var dailiesToDelete = entry.EmployeeScheduleDays.ToArray();
                foreach (var d in dailiesToDelete)
                {
                    var breaksToDelete = d.Breaks.ToArray();
                    foreach (var _break in breaksToDelete) _employeeScheduleDailyBreakRepository.Delete(_break);
                    //
                    _employeeScheduleDailyRepository.Delete(d);
                }
                _employeeScheduleRepository.Delete(entry);
            }
        }
        public void SaveEmployeeSchedule(EmployeeSchedule entity, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings)
        {
            errorsAndWarnings = null;
            if (entity.Id == 0)
            {
                InsertEmployeeSchedule(entity, out errorsAndWarnings);
            }
            else if (entity.CompanyShiftId == 0 || entity.JobRoleId == 0)
            {
                DeleteEmployeeSchedule(entity.Id);
            }
            else
            {
                UpdateEmployeeSchedule(entity, out errorsAndWarnings);
            }
            //
            if (errorsAndWarnings != null && errorsAndWarnings.Any())
            {
                errorsAndWarnings = errorsAndWarnings.Distinct();
            }
            else
            {
                UpdateScheduleJobOrderFromScheduleChange(entity.Id);
            }
        }
        public int GetShiftVacancyInPeriod(int companyShiftId, int schedulePeriodId, int jobRoleId, int? excludeEmployeeId, bool requiredOnly = false)
        {
            var companyShift = _companyShiftRepository.Table
                .Include(x => x.CompanyShiftJobRoles)
                .Where(x => x.Id == companyShiftId)
                .FirstOrDefault();
            if (companyShift != null)
            {
                var entries = _employeeScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId && x.JobRoleId == jobRoleId && !x.ForDailyAdhoc
                    && x.SchedulePeriodId == schedulePeriodId && (!excludeEmployeeId.HasValue || x.EmployeeId != excludeEmployeeId));
                var places = companyShift.CompanyShiftJobRoles.Where(x => x.CompanyJobRoleId == jobRoleId)
                    .Sum(x => (requiredOnly ? 0 : x.ContingencyRequiredCount) + x.MandantoryRequiredCount);
                return places - entries.Count();
            }
            return 0;
        }
        #endregion

        #region Calendar View
        public IEnumerable<IShiftViewDay> GetDailyScheduleForShiftView(int schedulePeriodId)
        {
            var scheduleDays = _shiftScheduleDailyRepository.TableNoTracking
                    .Include(y => y.CompanyShift.Shift)
                    .Where(x => x.SchedulePeriodId == schedulePeriodId)
                    .Where(x => (x.StartTimeOfDayTicks > 0 || x.LengthInHours > 1))
                    .OrderBy(d => d.ScheduelDate).ThenBy(d => d.CompanyShiftId);
            foreach (var day in scheduleDays)
            {
                var entry = EngineContext.Current.Resolve<IShiftViewDay>();
                entry.Shift = day;
                var schedules = _employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == day.CompanyShiftId)
                            .Select(x => new { Id = x.Id, Employee = x.Employee, JobRoleId = x.JobRoleId, ForDailyAdhoc = x.ForDailyAdhoc, Title = x.JobRole.Name }).GroupBy(x => x.Employee.Id)
                            .Select(g => g.FirstOrDefault()).ToArray();
                entry.EmployeeSchedules = schedules.OrderBy(x => x.Employee.FirstName + x.Employee.LastName).Select(x =>
                {
                    var item = EngineContext.Current.Resolve<IEmployeeScheduleViewModel>();
                    item.EmployeeScheduleId = x.Id;
                    item.Employee = x.Employee;
                    item.CompanyShiftId = day.CompanyShiftId;
                    item.JobRoleId = x.JobRoleId;
                    item.ForDailyAdhoc = x.ForDailyAdhoc;
                    item.Title = x.Title;
                    return item;
                });
                //
                yield return entry;
            }
        }
        public IEnumerable<EmployeeScheduleDaily> GetEmployeeScheduleDaily(int schedulePeriodId)
        {
            return _employeeScheduleDailyRepository.TableNoTracking
                .Include(x => x.EmployeeSchedule.Employee)
                .Include(x => x.EmployeeSchedule.JobRole)
                .Include(x => x.ReplacementCompanyJobRole)
                .Where(x => x.EmployeeSchedule.SchedulePeriodId == schedulePeriodId);
        }
        public IEnumerable<IScheduleDetailErrorModel> ValidateEmployeeScheduleDaily(EmployeeScheduleDaily entity, int schedulePeriodId = 0)
        {
            var master = _employeeScheduleRepository.Table.Where(x => x.Id == entity.EmployeeScheduleId).FirstOrDefault();
            if (master != null)
            {
                return GetScheduleErrorAndWarning(master.SchedulePeriodId, entity.ScheduelDate, entity.StartTimeOfDayTicks,
                    entity.EndTimeOfDay.Ticks, entity.ReplacementCompanyJobRoleId ?? master.JobRoleId,
                    entity.ReplacementEmployeeId ?? master.EmployeeId, master.CompanyShiftId, master.Id);
            }
            else
            {
                var shift = GetShiftsOfSchedulePeriod(schedulePeriodId).First();
                return GetScheduleErrorAndWarning(schedulePeriodId, entity.ScheduelDate, entity.StartTimeOfDayTicks,
                    entity.EndTimeOfDay.Ticks, entity.ReplacementCompanyJobRoleId.Value, entity.ReplacementEmployeeId.Value, shift.CompanyShiftId, 0);
            }
        }
        public EmployeeScheduleDaily UpdateEmployeeScheduleDaily(EmployeeScheduleDaily entity)
        {
            var entry = _employeeScheduleDailyRepository.Table.Where(x => x.EmployeeScheduleId == entity.EmployeeScheduleId
                && x.ScheduelDate == entity.ScheduelDate)
                .Include(x => x.EmployeeSchedule)
                .Include(x => x.Breaks).FirstOrDefault();
            if (entry != null)
            {
                var breaksToDelete = entry.Breaks.ToArray();
                foreach (var _break in breaksToDelete) _employeeScheduleDailyBreakRepository.Delete(_break);
                foreach (var _break in entity.Breaks)
                {
                    _break.EmployeeScheduleDaily = entry;
                    _break.EmployeeScheduleDailyId = entry.Id;
                }
                entry.Breaks = entity.Breaks;
                //
                entry.StartTimeOfDayTicks = entity.StartTimeOfDayTicks;
                entry.LengthInHours = entity.LengthInHours;
                entry.ReplacementEmployeeId = entry.EmployeeSchedule.EmployeeId == entity.ReplacementEmployeeId.GetValueOrDefault() ? null : entity.ReplacementEmployeeId;
                entry.ReplacementCompanyJobRoleId = entity.ReplacementCompanyJobRoleId;
                _employeeScheduleDailyRepository.Update(entry);
            }
            else
            {
                _employeeScheduleDailyRepository.Insert(entity);
            }
            entry = _employeeScheduleDailyRepository.Table.Where(x => x.EmployeeScheduleId == entity.EmployeeScheduleId
                && x.ScheduelDate == entity.ScheduelDate)
                .Include(x => x.EmployeeSchedule.Employee)
                .Include(x => x.ReplacementEmployee).FirstOrDefault();
            UpdateScheduleJobOrderFromScheduleChange(entry.EmployeeScheduleId);
            return entry;
        }
        public string UpdateEmployeeBreakTimePosition(int employeeScheduleId, DateTime scheduleDate, int breakIndex, int breakTimePosition)
        {
            var entry = _employeeScheduleDailyRepository.Table.Where(x => x.EmployeeScheduleId == employeeScheduleId
                && x.ScheduelDate == scheduleDate)
                .Include(x => x.Breaks).FirstOrDefault();
            if (entry != null)
            {
                var breaks = entry.Breaks.OrderBy(x => x.BreakTimeOfDayTicks);
                EmployeeScheduleDailyBreak _break;
                if (breaks.Count() > breakIndex)
                {
                    _break = breaks.Skip(breakIndex).First();
                }
                else
                {
                    _break = new EmployeeScheduleDailyBreak
                    {
                        EmployeeScheduleDailyId = entry.Id,
                    };
                    entry.Breaks.Add(_break);
                }
                _break.BreakTimeOfDayTicks = entry.StartTimeOfDayTicks +
                    (entry.EndTimeOfDay - entry.StartTimeOfDay).Ticks * breakTimePosition / 100;
                _employeeScheduleDailyRepository.Update(entry);
                return DateTime.Today.Add(_break.BreakTimeOfDay).ToString("hh:mm tt");
            }
            else
            {
                var templateEntry = _employeeScheduleRepository.Table.Where(x => x.Id == employeeScheduleId)
                    .Include(x => x.CompanyShift.ShiftSchedules)
                    .FirstOrDefault();
                if (templateEntry != null)
                {
                    var template = templateEntry.CompanyShift
                            .ShiftSchedules.Where(x => x.SchedulePeriodId == templateEntry.SchedulePeriodId).FirstOrDefault();
                    if (template != null)
                    {
                        var entity = new EmployeeScheduleDaily
                        {
                            EmployeeScheduleId = employeeScheduleId,
                            ScheduelDate = scheduleDate,
                            StartTimeOfDayTicks = template.StartTimeOfDayTicks,
                            LengthInHours = template.LengthInHours,
                            Breaks = new List<EmployeeScheduleDailyBreak>(),
                        };
                        var firstBreak = new EmployeeScheduleDailyBreak();
                        entity.Breaks.Add(firstBreak);
                        firstBreak.BreakTimeOfDayTicks = entity.StartTimeOfDayTicks +
                                (entity.EndTimeOfDay - entity.StartTimeOfDay).Ticks * breakTimePosition / 100;
                        firstBreak.BreakLengthInMinutes = 30m;
                        _employeeScheduleDailyRepository.Insert(entity);
                        return DateTime.Today.Add(firstBreak.BreakTimeOfDay).ToString("hh:mm tt");
                    }
                }
            }
            return string.Empty;
        }
        public void DeleteEmployeeScheduleDaily(int employeeScheduleId, DateTime scheduleDate)
        {
            var entry = _employeeScheduleDailyRepository.Table.Where(x => x.EmployeeScheduleId == employeeScheduleId
                && x.ScheduelDate == scheduleDate).FirstOrDefault();
            if (entry != null)
            {
                entry.IsDeleted = true;
                _employeeScheduleDailyRepository.Update(entry);
            }
            else
            {
                var entity = new EmployeeScheduleDaily
                {
                    EmployeeScheduleId = employeeScheduleId,
                    ScheduelDate = scheduleDate,
                    IsDeleted = true,
                };
                _employeeScheduleDailyRepository.Insert(entity);
            }
        }
        private void EnsureShiftScheduleDaily(int schedulePeriodId, int companyShiftId, DateTime scheduleDate)
        {
            var entry = _shiftScheduleDailyRepository.Table
                .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == companyShiftId && x.ScheduelDate == scheduleDate)
                .FirstOrDefault();
            if (entry == null)
            {
                entry = new ShiftScheduleDaily
                {
                    SchedulePeriodId = schedulePeriodId,
                    CompanyShiftId = companyShiftId,
                    ScheduelDate = scheduleDate,
                    StartTimeOfDayTicks = 0,
                    LengthInHours = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                };
                _shiftScheduleDailyRepository.Insert(entry);
            }
        }
        public int AddAdhocEmployeeSchedule(int schedulePeriodId, int employeeId, DateTime scheduleDate)
        {
            var shift = GetShiftsOfSchedulePeriod(schedulePeriodId).First();

            var entry = new EmployeeSchedule
            {
                SchedulePeriodId = schedulePeriodId,
                EmployeeId = employeeId,
                CompanyShiftId = shift.CompanyShiftId,
                JobRoleId = shift.CompanyShift.CompanyShiftJobRoles.First().CompanyJobRoleId,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ForDailyAdhoc = true,
            };
            EnsureShiftScheduleDaily(schedulePeriodId, shift.CompanyShiftId, scheduleDate);
            _employeeScheduleRepository.Insert(entry);
            return entry.Id;
        }
        #endregion

        #region Vacancy View
        private IEnumerable<Employee> GetScheduleEmployees(int schedulePeriodId, int companyShiftId, int companyJobRoleId)
        {
            var query = (_employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == companyShiftId
                                && x.JobRoleId == companyJobRoleId && !x.ForDailyAdhoc)
                            .Select(x => x.Employee)
                            .Union(_employeeScheduleDailyRepository.TableNoTracking
                            .Where(x => x.EmployeeSchedule.SchedulePeriodId == schedulePeriodId && x.EmployeeSchedule.CompanyShiftId == companyShiftId
                                && (x.EmployeeSchedule.JobRoleId == companyJobRoleId || x.ReplacementCompanyJobRoleId == companyJobRoleId))
                                .Where(x => x.ReplacementEmployeeId.HasValue && (x.EmployeeSchedule.EmployeeId != x.ReplacementEmployeeId
                                    || x.EmployeeSchedule.ForDailyAdhoc || (x.EmployeeSchedule.JobRoleId != companyJobRoleId && x.ReplacementCompanyJobRoleId == companyJobRoleId))
                                    && !x.IsDeleted && x.LengthInHours > 0)
                                .Select(x => x.ReplacementEmployee))).GroupBy(x => x.Id)
                            .Select(g => g.FirstOrDefault()).Where(x => x != null);
            return query.ToArray();
        }
        private decimal GetPlannedHours(EmployeeSchedule eSchedule, ShiftSchedule shiftSchedule, IEnumerable<DateTime> scheduleDates)
        {
            var result = 0m;
            foreach (var date in scheduleDates)
            {
                var overrides = eSchedule.EmployeeScheduleDays.Where(x => x.ScheduelDate == date &&
                    (!x.ReplacementEmployeeId.HasValue || x.ReplacementEmployeeId == eSchedule.EmployeeId)).FirstOrDefault();
                if (overrides != null && !overrides.IsDeleted)
                {
                    result += overrides.LengthInHours;
                }
                else if (overrides == null)
                {
                    result += shiftSchedule.LengthInHours;
                }
            }
            return result;
        }

        private IEnumerable<KeyValuePair<int, decimal>> MergeEmployeeHours(IEnumerable<KeyValuePair<int, decimal>> source1, IEnumerable<KeyValuePair<int, decimal>> source2)
        {
            return source1.Union(source2)
                .GroupBy(x => x.Key)
                .Select(g => new KeyValuePair<int, decimal>(g.Key, g.Sum(y => y.Value)));
        }

        public IEnumerable<IVacancyViewModel> GetScheduleVacancyView(int schedulePeriodId)
        {
            var period = _schedulePeriodRepository.TableNoTracking
                .Include(x => x.ShiftSchedules.Select(y => y.CompanyShift.CompanyShiftJobRoles.Select(r => r.CompanyJobRole)))
                .Include(x => x.ShiftSchedules.Select(y => y.CompanyShift.Shift))
                .Where(x => x.Id == schedulePeriodId).FirstOrDefault();
            var shiftSchedules = _FilterShiftSchedulesByAccount(period.ShiftSchedules);
            foreach (var shiftSchedule in shiftSchedules.Where(s => s.LengthInHours > 0).OrderBy(s => s.StartTimeOfDay))
            {
                var entry = EngineContext.Current.Resolve<IVacancyViewModel>();
                entry.Shift = shiftSchedule;
                //
                var dateList = _shiftScheduleDailyRepository.TableNoTracking
                    .Where(x => x.SchedulePeriodId == schedulePeriodId)
                    .Where(x => x.CompanyShiftId == shiftSchedule.CompanyShiftId)
                    .Where(x => x.LengthInHours > 0)
                    .Select(x => x.ScheduelDate);
                if (dateList.Any())
                {
                    entry.ScheduledDates = dateList.ToArray();
                    //
                    var vacancyList = new List<IRoleScheduleViewModel>();
                    foreach (var role in shiftSchedule.CompanyShift.CompanyShiftJobRoles)
                    {
                        var roleVM = EngineContext.Current.Resolve<IRoleScheduleViewModel>();
                        roleVM.JobRole = role.CompanyJobRole;
                        roleVM.ScheduledEmployees = GetScheduleEmployees(schedulePeriodId, shiftSchedule.CompanyShiftId, role.CompanyJobRoleId);
                        roleVM.Vacancy = GetShiftVacancyInPeriod(shiftSchedule.CompanyShiftId, schedulePeriodId, role.CompanyJobRole.Id, null);
                        roleVM.Published = _employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == shiftSchedule.CompanyShiftId && x.JobRoleId == role.CompanyJobRoleId)
                            .Any(x => x.PublishedJobOrderId.HasValue);
                        //
                        var totalHours = shiftSchedule.TotalHours * (role.MandantoryRequiredCount + role.ContingencyRequiredCount);
                        var regularPlannedHours = _employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == shiftSchedule.CompanyShiftId && x.JobRoleId == role.CompanyJobRoleId && !x.ForDailyAdhoc)
                            .Include(x => x.EmployeeScheduleDays)
                            .ToArray()
                            .Select(x => new KeyValuePair<int, decimal>(x.EmployeeId, GetPlannedHours(x, shiftSchedule, entry.ScheduledDates)));
                        var deletedRegularPlannedHours = _employeeScheduleDailyRepository.TableNoTracking
                            .Where(x => x.EmployeeSchedule.SchedulePeriodId == schedulePeriodId && x.EmployeeSchedule.CompanyShiftId == shiftSchedule.CompanyShiftId && x.EmployeeSchedule.JobRoleId == role.CompanyJobRoleId)
                            .Where(x => x.IsDeleted && !x.EmployeeSchedule.ForDailyAdhoc).ToArray()
                            .Select(x => new KeyValuePair<int, decimal>(x.EmployeeSchedule.EmployeeId, -x.LengthInHours));
                        var irregularPlannedHours = _employeeScheduleDailyRepository.TableNoTracking
                            .Where(x => x.EmployeeSchedule.SchedulePeriodId == schedulePeriodId && x.EmployeeSchedule.CompanyShiftId == shiftSchedule.CompanyShiftId && 
                                (x.EmployeeSchedule.JobRoleId == role.CompanyJobRoleId || x.ReplacementCompanyJobRoleId == role.CompanyJobRoleId))
                            .Where(x => x.ReplacementEmployeeId.HasValue && (x.EmployeeSchedule.EmployeeId != x.ReplacementEmployeeId
                                    || x.EmployeeSchedule.ForDailyAdhoc || (x.EmployeeSchedule.JobRoleId != role.CompanyJobRoleId 
                                    && x.ReplacementCompanyJobRoleId == role.CompanyJobRoleId))
                                    && !x.IsDeleted && x.LengthInHours > 0)
                            .ToArray()
                            .Select(x => new KeyValuePair<int, decimal>(x.ReplacementEmployeeId.Value, x.LengthInHours));
                        roleVM.PlannedHours = MergeEmployeeHours(MergeEmployeeHours(regularPlannedHours, irregularPlannedHours), deletedRegularPlannedHours);
                        roleVM.UnplannedHours = totalHours - roleVM.PlannedHours.Sum(x => x.Value);
                        //
                        vacancyList.Add(roleVM);
                    }
                    entry.RoleVacancy = vacancyList.ToArray();
                    yield return entry;
                }
            }
        }

        private IEnumerable<ShiftSchedule> _FilterShiftSchedulesByAccount(IEnumerable<ShiftSchedule> shiftSchedules)
        {
            var account = _workContext.CurrentAccount;

            if (account.IsCompanyLocationManager())
                shiftSchedules = shiftSchedules.Where(x => x.CompanyShift.CompanyDepartment.CompanyLocationId == account.CompanyLocationId);

            else if (account.IsCompanyDepartmentManager())
                shiftSchedules = shiftSchedules.Where(x => x.CompanyShift.CompanyDepartmentId == account.CompanyDepartmentId);

            else if (account.IsCompanyDepartmentSupervisor())
                shiftSchedules = shiftSchedules.Where(x => x.CompanyShift.CompanyShiftJobRoles.Any(y => y.SupervisorId == account.Id));

            return shiftSchedules;
        }

        private IQueryable<Employee> FilterEmployees(IQueryable<Employee> query)
        {
            var account = _workContext.CurrentAccount;
            query = query.Where(cwt => cwt.CompanyId == account.CompanyId);
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) {; }

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId.HasValue &&
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId); // search within locatin

            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId.HasValue &&
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId); //&&
                                                                         //cwt.CompanyDepartmentId > 0 &&
                                                                         //cwt.CompanyDepartmentId == account.CompanyDepartmentId &&   // search within department
                                                                         //cwt.ManagerId. == account.Id);
            else if (account.IsCompanyDepartmentManager())
                query = query.Where(cwt =>
                    cwt.CompanyLocationId > 0 &&
                    cwt.CompanyLocationId == account.CompanyLocationId);// &&
                                                                        //cwt.CompanyDepartmentId > 0 &&
                                                                        //cwt.CompanyDepartmentId == account.CompanyDepartmentId);
            return query;
        }

        public IEnumerable<Employee> GetAvailableEmployees(int? schedulePeriodId, int? companyShiftId, int? companyJobRoleId)
        {
            var scheduledEmployeeIds = new int[] { };// _employeeScheduleRepository.TableNoTracking
                                                     //.Where(x => x.SchedulePeriodId == schedulePeriodId)// && x.CompanyShiftId == companyShiftId)
                                                     //.Select(x => x.EmployeeId).ToArray();
                                                     //var schedulePeriod = _schedulePeriodRepository.GetById(schedulePeriodId);
                                                     //if (schedulePeriod == null) return new Employee[] { };
                                                     //var periodStartLocal = schedulePeriod.PeriodStartUtc.ToLocalTime();
                                                     //var periodEndLocal = schedulePeriod.PeriodEndUtc.ToLocalTime();
                                                     //
            var query = _employeeRepository.Table
                .Where(x => !scheduledEmployeeIds.Contains(x.Id) && x.IsActive && !x.IsDeleted)
                .Where(x => !companyJobRoleId.HasValue || x.EmployeeJobRoles.Any(r => r.CompanyJobRoleId == companyJobRoleId));
            //.Where(x => !x.EmployeeTimeoffBookings.Any(b => (b.TimeOffStartDateTime >= periodStartLocal && b.TimeOffStartDateTime <= periodEndLocal) ||
            //    (b.TimeOffEndDateTime >= periodStartLocal && b.TimeOffEndDateTime <= periodEndLocal)));

            //
            return FilterEmployees(query).OrderBy(x => x.FirstName + x.LastName);
        }

        public void DeleteEmployeeSchedule(int employeeId, int schedulePeriodId, int companyShiftId)
        {
            var entitiesToDelete = _employeeScheduleRepository.TableNoTracking
                    .Include(x => x.EmployeeScheduleDays)
                    .Where(x => x.SchedulePeriodId == schedulePeriodId && x.CompanyShiftId == companyShiftId && x.EmployeeId == employeeId).ToArray();
            foreach (var e in entitiesToDelete)
            {
                var dailiesToDelete = e.EmployeeScheduleDays.ToArray();
                foreach (var d in dailiesToDelete)
                {
                    _employeeScheduleDailyRepository.Delete(d);

                }
                _employeeScheduleRepository.Delete(e);
            }
        }
        #endregion

        #region Employee Schedule
        public IEnumerable<IShiftViewDay> GetEmployeeScheduleBaseline(Guid employeeGuid)
        {
            var scheduleDays = _shiftScheduleDailyRepository.TableNoTracking
                    .Include(y => y.CompanyShift.Shift)
                    .Where(x => x.CompanyShift.EmployeeSchedules.Any(es => es.Employee.CandidateGuid == employeeGuid))
                    .Where(x => (x.StartTimeOfDayTicks > 0 || x.LengthInHours > 1))
                    .OrderBy(d => d.ScheduelDate).ThenBy(d => d.CompanyShiftId);
            foreach (var day in scheduleDays)
            {
                var entry = EngineContext.Current.Resolve<IShiftViewDay>();
                entry.Shift = day;
                var schedules = _employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == day.SchedulePeriodId && x.CompanyShiftId == day.CompanyShiftId)
                            .Where(x => x.Employee.CandidateGuid == employeeGuid)
                            .Select(x => new { Id = x.Id, Employee = x.Employee, JobRoleId = x.JobRoleId, ForDailyAdhoc = x.ForDailyAdhoc, Title = x.JobRole.Name })
                            .GroupBy(x => x.Employee.Id)
                            .Select(g => g.FirstOrDefault()).ToArray();
                entry.EmployeeSchedules = schedules.OrderBy(x => x.Employee.FirstName + x.Employee.LastName).Select(x =>
                {
                    var item = EngineContext.Current.Resolve<IEmployeeScheduleViewModel>();
                    item.EmployeeScheduleId = x.Id;
                    item.Employee = x.Employee;
                    item.CompanyShiftId = day.CompanyShiftId;
                    item.JobRoleId = x.JobRoleId;
                    item.ForDailyAdhoc = x.ForDailyAdhoc;
                    item.Title = x.Title;
                    return item;
                });
                //
                yield return entry;
            }
        }
        public IEnumerable<EmployeeScheduleDaily> GetEmployeeScheduleOverride(Guid employeeGuid)
        {
            return _employeeScheduleDailyRepository.TableNoTracking
                .Include(x => x.EmployeeSchedule.Employee)
                .Include(x => x.ReplacementCompanyJobRole)
                .Include(x => x.EmployeeSchedule.JobRole)
                .Where(x => x.EmployeeSchedule.Employee.CandidateGuid == employeeGuid);
        }
        public IEnumerable<IShiftViewDay> GetTeamScheduleBaseline(IEnumerable<Guid> employeeGuids)
        {
            var scheduleDays = _shiftScheduleDailyRepository.TableNoTracking
                    .Include(y => y.CompanyShift.Shift)
                    .Where(x => x.CompanyShift.EmployeeSchedules.Any(es => employeeGuids.Contains(es.Employee.CandidateGuid)))
                    .Where(x => (x.StartTimeOfDayTicks > 0 || x.LengthInHours > 1))
                    .OrderBy(d => d.ScheduelDate).ThenBy(d => d.CompanyShiftId);
            foreach (var day in scheduleDays)
            {
                var entry = EngineContext.Current.Resolve<IShiftViewDay>();
                entry.Shift = day;
                var schedules = _employeeScheduleRepository.TableNoTracking
                            .Where(x => x.SchedulePeriodId == day.SchedulePeriodId && x.CompanyShiftId == day.CompanyShiftId)
                            .Where(x => employeeGuids.Contains(x.Employee.CandidateGuid))
                            .Select(x => new { Id = x.Id, Employee = x.Employee, JobRoleId = x.JobRoleId, ForDailyAdhoc = x.ForDailyAdhoc, Title = x.JobRole.Name })
                            .GroupBy(x => x.Employee.Id)
                            .Select(g => g.FirstOrDefault()).ToArray();
                entry.EmployeeSchedules = schedules.OrderBy(x => x.Employee.FirstName + x.Employee.LastName).Select(x =>
                {
                    var item = EngineContext.Current.Resolve<IEmployeeScheduleViewModel>();
                    item.EmployeeScheduleId = x.Id;
                    item.Employee = x.Employee;
                    item.CompanyShiftId = day.CompanyShiftId;
                    item.JobRoleId = x.JobRoleId;
                    item.ForDailyAdhoc = x.ForDailyAdhoc;
                    item.Title = x.Title;
                    return item;
                });
                //
                yield return entry;
            }
        }
        public IEnumerable<EmployeeScheduleDaily> GetTeamScheduleOverride(IEnumerable<Guid> employeeGuids)
        {
            return _employeeScheduleDailyRepository.TableNoTracking
                .Include(x => x.EmployeeSchedule.Employee)
                .Include(x => x.ReplacementCompanyJobRole)
                .Include(x => x.EmployeeSchedule.JobRole)
                .Include(x => x.ReplacementEmployee)
                .Where(x => employeeGuids.Contains(x.ReplacementEmployee.CandidateGuid));
        }
        public void UpdateJobOrderFromEmployeeSchedules(IEnumerable<EmployeeSchedule> entriesToPublish)
        {
            List<int> jobOrderProcessed = new List<int>();
            foreach (var entry in entriesToPublish)
            {
                var jobOrder = FindJobOrder(entry.SchedulePeriodId, entry.CompanyShiftId, entry.JobRoleId);
                if (jobOrder != null && !jobOrderProcessed.Contains(jobOrder.Id))
                {
                    var currentPipeline = _candidateJobOrderRepository.Table.Where(x => x.JobOrderId == jobOrder.Id).ToArray();
                    foreach (var item in currentPipeline)
                    {
                        _candidateJobOrderRepository.Delete(item);
                    }
                    jobOrder.JobOrderStatusId = (int)(JobOrderStatusEnum.Active);
                    entry.PublishedJobOrderId = jobOrder.Id;
                    _employeeScheduleRepository.Update(entry);
                    //
                    var employees = GetScheduleEmployees(entry.SchedulePeriodId, entry.CompanyShiftId, entry.JobRoleId);
                    foreach(var employee in employees)
                    {
                        var cEntry = new CandidateJobOrder
                        {
                            JobOrderId = jobOrder.Id,
                            CandidateId = employee.Id,
                            StartDate = entry.SchedulePeriod.PeriodStartUtc.ToLocalTime(),
                            EndDate = entry.SchedulePeriod.PeriodEndUtc.ToLocalTime(),
                            CandidateJobOrderStatusId = (int)(CandidateJobOrderStatusEnum.Submitted),
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                            EnteredBy = _workContext.CurrentAccount.Id,
                            Note = "Submitted via Schedule Publishing",
                        };
                        _candidateJobOrderRepository.Insert(cEntry);
                    }
                    //
                    jobOrderProcessed.Add(jobOrder.Id);
                }
            }
        }
        public void UpdateScheduleJobOrderFromScheduleChange(int employeeScheduleId)
        {
            var employeeSchedule = _employeeScheduleRepository.TableNoTracking
                .Where(x => x.Id == employeeScheduleId).First();
            var currentPipeline = _employeeScheduleRepository.Table
                .Where(x => x.SchedulePeriodId == employeeSchedule.SchedulePeriodId &&
                x.CompanyShiftId == employeeSchedule.CompanyShiftId &&
                x.JobRoleId == employeeSchedule.JobRoleId)
                .Include(x => x.SchedulePeriod).ToArray();
            UpdateJobOrderFromEmployeeSchedules(currentPipeline);
        }
        #endregion

        public IList<Employee> GetEmployeeListForScheduleFilter()
        {
            return GetAvailableEmployees(null, null, null).ToList();
        }
    }
}
