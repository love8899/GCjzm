using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Infrastructure;
using Wfm.Services.Configuration;

namespace Wfm.Services.Scheduling
{
    public class AutoScheduleService : IAutoScheduleService
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<SchedulePeriod> _schedulePeriodRepository;
        private readonly IRepository<ShiftSchedule> _shiftScheduleRepository;
        private readonly IRepository<ShiftScheduleDaily> _shiftScheduleDailyRepository;
        private readonly IRepository<CompanyShift> _companyShiftRepository;
        private readonly IRepository<EmployeeSchedule> _employeeScheduleRepository;
        private readonly IRepository<EmployeeScheduleDaily> _employeeScheduleDailyRepository;
        private readonly IRepository<EmployeeAvailability> _employeeAvailabilityRepository;
        private readonly IRepository<CompanyOvertimeRule> _companyOvertimeRuleRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<CandidateJobOrder> _candidateJobOrderRepository;
        private readonly IRepository<ScheduleJobOrder> _scheduleJobOrderRepository;
        private readonly ISettingService _settingService;
        private readonly ISchedulingDemandService _scheduleDemandingService;
        private readonly string[] ResourceColors = new string[] {
                    "#E1F5FE", "#E0F7FA", "#E3F2FD", "#E0F2F1",
                    "#B3E5FC", "#B2EBF2", "#BBDEFB", "#B2DFDB",
                    "#81D4FA", "#80DEEA", "#90CAF9", "#80CBC4",
                    "#4FC3F7", "#4DD0E1", "#64B5F6", "#4DB6AC",
                    "#29B6F6", "#26C6DA", "#42A5F5", "#26A69A"
                };
        #endregion

        #region ctor
        public AutoScheduleService(IWorkContext workContext,
            IRepository<Employee> employeeRepository,
            IRepository<SchedulePeriod> schedulePeriodRepository,
            IRepository<ShiftSchedule> shiftScheduleRepository,
            IRepository<ShiftScheduleDaily> shiftScheduleDailyRepository,
            IRepository<CompanyShift> companyShiftRepository,
            IRepository<EmployeeSchedule> employeeScheduleRepository,
            IRepository<EmployeeScheduleDaily> employeeScheduleDailyRepository,
            IRepository<EmployeeAvailability> employeeAvailabilityRepository,
            IRepository<CompanyOvertimeRule> companyOvertimeRuleRepository,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<CandidateJobOrder> candidateJobOrderRepository,
            IRepository<ScheduleJobOrder> scheduleJobOrderRepository,
            ISettingService settingService,
            ISchedulingDemandService scheduleDemandingService)
        {
            _workContext = workContext;
            _employeeRepository = employeeRepository;
            _schedulePeriodRepository = schedulePeriodRepository;
            _shiftScheduleRepository = shiftScheduleRepository;
            _shiftScheduleDailyRepository = shiftScheduleDailyRepository;
            _companyShiftRepository = companyShiftRepository;
            _employeeScheduleRepository = employeeScheduleRepository;
            _employeeScheduleDailyRepository = employeeScheduleDailyRepository;
            _employeeAvailabilityRepository = employeeAvailabilityRepository;
            _companyOvertimeRuleRepository = companyOvertimeRuleRepository;
            _jobOrderRepository = jobOrderRepository;
            _candidateJobOrderRepository = candidateJobOrderRepository;
            _scheduleJobOrderRepository = scheduleJobOrderRepository;
            _settingService = settingService;
            _scheduleDemandingService = scheduleDemandingService;
        }
        #endregion

        #region Public Methods
        public bool AutoFillupVacancy(int schedulePeriodId, int companyShiftId, bool ignoreWarning, bool priorityToRecentEmployee, out IEnumerable<IScheduleDetailErrorModel> warnings)
        {
            var rand = new Random(DateTime.Now.Second);
            //var vacancyView = _scheduleDemandingService.GetScheduleVacancyView(schedulePeriodId).OrderByDescending(x => x.Shift.TotalHours);
            var vacancyView = _scheduleDemandingService.GetScheduleVacancyView(schedulePeriodId).Where(x => x.Shift.CompanyShiftId == companyShiftId);
            warnings = new List<IScheduleDetailErrorModel>();
            foreach (var entry in vacancyView)
            {
                var rolesToFill = entry.RoleVacancy.Where(r => r.Vacancy > 0).ToArray();
                foreach (var role in rolesToFill)
                {
                    var vacancy = role.Vacancy;
                    var jobRoleId = role.JobRole.Id;
                    var triedEmployeeIds = new List<int>();
                    while (vacancy > 0)
                    {
                        var employeeQueue = _scheduleDemandingService.GetAvailableEmployees(schedulePeriodId, entry.Shift.CompanyShiftId, null)
                            .Where(x => !triedEmployeeIds.Contains(x.Id)).ToArray();
                        if (!employeeQueue.Any())
                        {
                            break;
                        }
                        var titleFitEmployees = _scheduleDemandingService.GetAvailableEmployees(schedulePeriodId, entry.Shift.CompanyShiftId, jobRoleId)
                            .Where(x => !triedEmployeeIds.Contains(x.Id)).ToArray();
                        Employee nextEmployee = titleFitEmployees.Any() ? titleFitEmployees[rand.Next(0, titleFitEmployees.Length)] : employeeQueue[rand.Next(0, employeeQueue.Length)];
                        IEnumerable<IScheduleDetailErrorModel> errorWarnings;
                        _scheduleDemandingService.InsertEmployeeSchedule(new EmployeeSchedule
                        {
                            EmployeeId = nextEmployee.Id,
                            CompanyShiftId = entry.Shift.CompanyShiftId,
                            JobRoleId = role.JobRole.Id,
                            SchedulePeriodId = schedulePeriodId,
                            Note = "Result of auto-scheduling",
                        }, out errorWarnings);
                        triedEmployeeIds.Add(nextEmployee.Id);
                        if (!errorWarnings.Any(e => e.LevelOfError == ScheduleWarningLevel.Error) && (ignoreWarning || !errorWarnings.Any()))
                        {
                            vacancy--;
                            //((List<IScheduleDetailErrorModel>)warnings).AddRange(errorWarnings);
                        }
                        else if (errorWarnings.Any(e => e.Scope == ScheduleWarningScope.Shift))
                        {
                            //shift issue, no need to try
                            break;
                        }
                        else
                        {
                            //((List<IScheduleDetailErrorModel>)warnings).AddRange(errorWarnings);
                        }
                    }
                }
            }
            warnings = warnings.Distinct();
            return true;
        }
        public void ResetEmployeeSchedule(int schedulePeriodId, int companyShiftId)
        {
            var entriesToRemove = _employeeScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId
                && x.SchedulePeriodId == schedulePeriodId).Include(x => x.EmployeeScheduleDays).ToArray();
            foreach (var entry in entriesToRemove)
            {
                var dailiesToDelete = entry.EmployeeScheduleDays.ToArray();
                foreach (var d in dailiesToDelete) _employeeScheduleDailyRepository.Delete(d);
            }
            _employeeScheduleRepository.Delete(entriesToRemove);
        }
        public void PublishEmployeeSchedule(int schedulePeriodId, int companyShiftId)
        {
            var schedulePeriod = _schedulePeriodRepository.Table.Include(x => x.ShiftSchedules).Where(x => x.Id == schedulePeriodId).First();
            _scheduleDemandingService.SyncJobOrders(schedulePeriod.ShiftSchedules.First(x => x.CompanyShiftId == companyShiftId));
            var entriesToPublish = _employeeScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId 
                && x.SchedulePeriodId == schedulePeriodId && !x.PublishedJobOrderId.HasValue)
                .Include(x => x.SchedulePeriod).ToArray();
            _scheduleDemandingService.UpdateJobOrderFromEmployeeSchedules(entriesToPublish);
        }

        public IEnumerable<IScheduleDetailErrorModel> MoveEmployeeSchedule(int schedulePeriodId, int fromCompanyShiftId, int fromCompanyRoleId, 
            int employeeId, int toCompanyShiftid, int toCompanyRoleId)
        {
            var entryToUpdate = _employeeScheduleRepository.Table.Where(x => x.CompanyShiftId == fromCompanyShiftId
                && x.SchedulePeriodId == schedulePeriodId && x.EmployeeId == employeeId && x.JobRoleId == fromCompanyRoleId).FirstOrDefault();
            IEnumerable<IScheduleDetailErrorModel> errorWarnings = null;
            if (entryToUpdate != null)
            {
                _scheduleDemandingService.DeleteEmployeeSchedule(entryToUpdate.Id);
                _scheduleDemandingService.InsertEmployeeSchedule(new EmployeeSchedule
                {
                    EmployeeId = employeeId,
                    CompanyShiftId = toCompanyShiftid,
                    JobRoleId = toCompanyRoleId,
                    SchedulePeriodId = schedulePeriodId,
                    Note = "Result of employee movement",
                }, out errorWarnings);
                if (errorWarnings.Any(x => x.LevelOfError == ScheduleWarningLevel.Error))
                {
                    // failed to move
                    _employeeScheduleRepository.Insert(entryToUpdate);
                }
            }
            //
            if (errorWarnings != null && errorWarnings.Any())
            {
                errorWarnings = errorWarnings.Distinct();
            }
            return errorWarnings;
        }
        public IEnumerable GetEmployeeResourceList()
        {
            var range = this.ResourceColors.Length;
            return _scheduleDemandingService.GetAvailableEmployees(null, null, null)
                .Select(x => new { Text = x.ToString(), Value = x.Id.ToString(), Color = this.ResourceColors[x.Id % range] });
        }
        #endregion

        #region Private Methods
        private CompanyJobRole DeterminJobRole(ICollection<EmployeeJobRole> employeeJobRoles, ICollection<CompanyShiftJobRole> companyShiftJobRoles)
        {
            var primaryEmployeeRole = employeeJobRoles.Where(x => x.IsPrimary).FirstOrDefault();
            if (primaryEmployeeRole != null && companyShiftJobRoles.Any(x => x.CompanyJobRoleId == primaryEmployeeRole.CompanyJobRoleId))
            {
                return primaryEmployeeRole.CompanyJobRole;
            }
            else
            {
                var employeeRoleIds = employeeJobRoles.Select(x => x.CompanyJobRoleId).ToArray();
                var firstMatch = companyShiftJobRoles.Where(x => employeeRoleIds.Contains(x.CompanyJobRoleId)).FirstOrDefault();
                if (firstMatch != null)
                {
                    return firstMatch.CompanyJobRole;
                }
                else if (companyShiftJobRoles.Any())
                {
                    return companyShiftJobRoles.First().CompanyJobRole;
                }
                else if (employeeJobRoles.Any())
                {
                    return employeeJobRoles.First().CompanyJobRole;
                }
            }
            return null;
        }
        #endregion

        #region Schedule Job Order
        public ScheduleJobOrder GetScheduleJobOrderByJobOrderId(int jobOrderId)
        {
            return _scheduleJobOrderRepository.Table
                .Where(x => x.JobOrderId == jobOrderId)
                .Include(x => x.JobOrder)
                .FirstOrDefault();
        }
        public void UpdateScheduleJobOrder(int scheduleJobOrderId, int? supervisorAccountId, DateTime startDate, DateTime? endDate, string jobTitle)
        {
            var entity = _scheduleJobOrderRepository.Table
                .Where(x => x.Id == scheduleJobOrderId)
                .Include(x => x.JobOrder)
                .FirstOrDefault();
            if (entity != null)
            {
                entity.SupervisorId = supervisorAccountId;
                entity.JobOrder.StartDate = startDate;
                entity.JobOrder.EndDate = endDate;
                entity.JobOrder.JobTitle = jobTitle;
                _scheduleJobOrderRepository.Update(entity);
            }
        }
        #endregion
    }
}
