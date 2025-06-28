using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Companies;
using Wfm.Services.Employees;
using Wfm.Services.Scheduling;


namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeePlacement_BL
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly IEmployeeService _employeeService;
        private readonly IRepository<ShiftSchedule> _shiftScheduleRepository;
        private readonly IRepository<ShiftScheduleDailyDemandAdjustment> _shiftScheduleDailyDemandAdjustmentRepository;
        private readonly IRepository<EmployeeSchedule> _employeeScheduleRepository;
        private readonly IRepository<CompanyShiftJobRole> _companyShiftJobRoleRepository;
        private readonly ISchedulingDemandService _schedulingDemandService;

        #endregion


        #region Ctor

        public EmployeePlacement_BL(
            ICompanyService companyService,
            IEmployeeService employeeService,
            IRepository<ShiftSchedule> shiftScheduleRepository,
            IRepository<ShiftScheduleDailyDemandAdjustment> shiftScheduleDailyDemandAdjustmentRepository,
            IRepository<EmployeeSchedule> employeeScheduleRepository,
            IRepository<CompanyShiftJobRole> companyShiftJobRoleRepository,
            ISchedulingDemandService schedulingDemandService)
        {
            _companyService = companyService;
            _employeeService =employeeService;
            _shiftScheduleRepository = shiftScheduleRepository;
            _shiftScheduleDailyDemandAdjustmentRepository = shiftScheduleDailyDemandAdjustmentRepository;
            _employeeScheduleRepository = employeeScheduleRepository;
            _companyShiftJobRoleRepository = companyShiftJobRoleRepository;
            _schedulingDemandService = schedulingDemandService;
        }

        #endregion

        
        #region Employee Placement

        public ShiftJobRoleModel GetShiftJobRoleModel(int shiftJobRoleId)
        {
            var shiftJobRole = _companyShiftJobRoleRepository.Table.Where(x => x.Id == shiftJobRoleId).FirstOrDefault();
            var companyShift = _companyService.GetCompanyShiftById(shiftJobRole.CompanyShiftId);
            var period = companyShift.CompanyDepartment != null ? _schedulingDemandService.GetSchedulePeriodByLocationAndDate(companyShift.CompanyDepartment.CompanyLocationId, companyShift.EffectiveDate):
                _schedulingDemandService.GetSchedulePeriodByCompanyAndDate(companyShift.CompanyId, companyShift.EffectiveDate);

            var model = new ShiftJobRoleModel()
            {
                Id = shiftJobRoleId,
                SchedulePeriodId = period.Id,
                PeriodStart = period.PeriodStartUtc.ToLocalTime(),
                PeriodEnd = period.PeriodEndUtc.ToLocalTime(),
                Department = companyShift.CompanyDepartment != null ? companyShift.CompanyDepartment.DepartmentName : string.Empty,
                CompanyJobRoleId = shiftJobRole.CompanyJobRoleId,
                JobRole = shiftJobRole.CompanyJobRole.Name,
                CompanyShiftId = companyShift.Id,
                Shift = companyShift.Shift.ShiftName,
                StartTime = DateTime.MinValue + companyShift.ShiftSchedules.FirstOrDefault().StartTimeOfDay,
                EndTime = (DateTime.MinValue + companyShift.ShiftSchedules.FirstOrDefault().StartTimeOfDay).AddHours((double)companyShift.ShiftSchedules.FirstOrDefault().LengthInHours),
                Planned = GetShiftPlannedNumbers(companyShift.Id, period)
            };

            return model;
        }

        public ShiftJobRoleModel GetShiftJobRoleModel(int schedulePeriodId, int companyShiftId, int jobRoleId)
        {
            var shiftJobRole = _companyShiftJobRoleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId && x.CompanyJobRoleId == jobRoleId).FirstOrDefault();
            var companyShift = _companyService.GetCompanyShiftById(companyShiftId);
            var period = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);

            var model = new ShiftJobRoleModel()
            {
                Id = shiftJobRole.Id,
                SchedulePeriodId = period.Id,
                PeriodStart = period.PeriodStartUtc.ToLocalTime(),
                PeriodEnd = period.PeriodEndUtc.ToLocalTime(),
                Department = companyShift.CompanyDepartment != null ? companyShift.CompanyDepartment.DepartmentName : string.Empty,
                CompanyJobRoleId = shiftJobRole.CompanyJobRoleId,
                JobRole = shiftJobRole.CompanyJobRole.Name,
                CompanyShiftId = companyShift.Id,
                Shift = companyShift.Shift.ShiftName,
                StartTime = DateTime.MinValue + companyShift.ShiftSchedules.FirstOrDefault().StartTimeOfDay,
                EndTime = (DateTime.MinValue + companyShift.ShiftSchedules.FirstOrDefault().StartTimeOfDay).AddHours((double)companyShift.ShiftSchedules.FirstOrDefault().LengthInHours),
                Planned = GetShiftPlannedNumbers(companyShift.Id, period)
            };

            return model;
        }

        public int[] GetShiftPlannedNumbers(int companyShiftId, SchedulePeriod period)
        {
            var companyShift = _companyService.GetCompanyShiftById(companyShiftId);
            var dates = Enumerable.Range(0, 7).Select(x => companyShift.EffectiveDate.AddDays(x));
            var schedules = _schedulingDemandService.GetDailyShiftSchedule(period.Id).Where(x => x.CompanyShiftId == companyShiftId);
            var ids = from d in dates
                      from s in schedules.Where(s => s.ScheduelDate == d).DefaultIfEmpty()
                      select s != null ? s.Id : 0;

            var adjustments = _shiftScheduleDailyDemandAdjustmentRepository.Table;
            var result = from i in ids
                         from a in adjustments.Where(a => a.ShiftScheduleDailyId == i).DefaultIfEmpty()
                         select a != null ? a.AdjustedMandantoryRequiredCount : 0;

            return result.ToArray();
        }


        public IQueryable<EmployeeSchedule> GetShiftEmployees(int companyShiftId)
        {
            var employees = _employeeScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId);

            return employees;
        }


        public List<SelectListItem> GetShiftEmployeesAsSelectList(int companyShiftId)
        {
            var companyId = _companyService.GetCompanyShiftById(companyShiftId).CompanyId;
            var scheduledEmployeeIds = GetShiftEmployees(companyShiftId).Select(x => x.EmployeeId);
            var employees = _employeeService.GetAllEmployeesByCompany(companyId, activeOnly: true).Where(x => scheduledEmployeeIds.Contains(x.Id))
                            .Select(x => new SelectListItem()
                            {
                                Text = x.FirstName + " " + x.LastName,
                                Value = x.Id.ToString()
                            });

            var result = new List<SelectListItem>();
            // Add dummy resource to prevent the undefined timeSlotRanges error 
            if (!employees.Any())
                result.Add(new SelectListItem() { Text = "", Value = "0"});
            result.AddRange(employees.ToList());

            return result;
        }


        public List<EmployeeSchedulePreviewModel> GetAllPlacementsByShift(int companyShiftId)
        {
            var employees = GetShiftEmployees(companyShiftId).ToList();

            var companyShift = _companyService.GetCompanyShiftById(companyShiftId);
            var shiftJobRoleId = companyShift.CompanyShiftJobRoles.FirstOrDefault().Id;
            var jobRole = companyShift.CompanyShiftJobRoles.FirstOrDefault().CompanyJobRole;
            //var SupervisorId = companyShift.ShiftSchedules.FirstOrDefault().SupervisorId.Value;

            var period = _schedulingDemandService.GetSchedulePeriodByLocationAndDate(companyShift.CompanyDepartment.CompanyLocationId, companyShift.EffectiveDate);
            var shiftSchedule = _shiftScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId).FirstOrDefault();
            var dailySchedules = _schedulingDemandService.GetDailyShiftSchedule(period.Id)
                                 .Where(x => x.CompanyShiftId == shiftSchedule.CompanyShiftId).ToList();

            var result = from e in employees
                         join s in dailySchedules on e.CompanyShiftId equals s.CompanyShiftId
                         select new EmployeeSchedulePreviewModel()
                         {
                             EmployeeScheduleId = e.Id,
                             SchedulePeriodId = period.Id,
                             CompanyShiftId = companyShift.Id,
                             CompanyJobRoleId = jobRole.Id,
                             EmployeeId = e.EmployeeId,
                             Title = companyShift.CompanyDepartment.DepartmentName,
                             Description = jobRole.Name,
                             Start = s.ScheduelDate + s.StartTimeOfDay,
                             End = (s.ScheduelDate + s.StartTimeOfDay).AddHours((double)s.LengthInHours),
                             BreakLengthInMinutes = new List<decimal?> { 30m },
                             IsAllDay = true
                         };


            return result.ToList();
        }


        public IEnumerable<IScheduleDetailErrorModel> PlaceEmployee(int jobRoleId, int companyShiftId, int employeeId)
        {
            var companyShift = _companyService.GetCompanyShiftById(companyShiftId);
            var period = _schedulingDemandService.GetSchedulePeriodByLocationAndDate(companyShift.CompanyDepartment.CompanyLocationId, companyShift.EffectiveDate);
            var entity = new EmployeeSchedule()
            {
                EmployeeId = employeeId,
                SchedulePeriodId = period.Id,
                JobRoleId = jobRoleId,
                CompanyShiftId = companyShiftId,
            };
            
            IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings = null;

            if (entity.Id == 0)
                _schedulingDemandService.InsertEmployeeSchedule(entity, out errorsAndWarnings);

            else if (entity.CompanyShiftId == 0 || entity.JobRoleId == 0)
                _schedulingDemandService.DeleteEmployeeSchedule(entity.Id);

            else
                _schedulingDemandService.UpdateEmployeeSchedule(entity, out errorsAndWarnings);

            if (errorsAndWarnings != null && errorsAndWarnings.Any())
                errorsAndWarnings = errorsAndWarnings.Distinct();

            return errorsAndWarnings;
        }

        #endregion
    }
}
