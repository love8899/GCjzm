using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Services.Scheduling
{
    public interface ISchedulingDemandService
    {
        #region Schedule Period
        IQueryable<SchedulePeriod> GetAllSchedulePeriods(int companyId);
        IQueryable<SchedulePeriod> GetAllSchedulePeriodsByAccount(Account account);
        decimal GetDefaultLengthInHours(int schedulePeriodId, int companyJobRoleId);
        IList<int> GetCompanyJobRoleIdsBySchedulePeriod(int schedulePeriodId);
        SchedulePeriod GetSchedulePeriodById(int id);
        void InsertCompanySchedulePeriod(SchedulePeriod model);
        void UpdateCompanySchedulePeriod(SchedulePeriod model);
        void DeleteCompanySchedulePeriod(int id);

        SchedulePeriod GetSchedulePeriodByCompanyAndDate(int companyId, DateTime refDate);
        SchedulePeriod GetSchedulePeriodByLocationAndDate(int locationId, DateTime refDate);

        IQueryable<SchedulePeriod> GetPreviousSchedulePeriodsByLocationAndDate(int locationId, DateTime refDate, bool validOnly = true);

        IQueryable<SchedulePeriod> GetLaterSchedulePeriodsByLocationAndDate(int locationId, DateTime refDate, bool validOnly = true);

        SchedulePeriod GetDemandByDemandDetails(int schedulePeriodId, int departmentId, int jobRoleId, TimeSpan startTime, decimal length);

        #endregion

        #region Schedule Shift
        IEnumerable<ShiftSchedule> GetShiftsOfSchedulePeriod(int schedulePeriodId);
        void UpdateShiftsOfSchedulePeriod(int schedulePeriodId, IEnumerable<ShiftSchedule> shifts);
        void CopySchedulePeriodAndShifts(int schedulePeriodId, DateTime toStartDate, DateTime toEndDate);
        void SyncJobOrders(ShiftSchedule shiftSchedule);
        JobOrder FindJobOrder(int schedulePeriodId, int companyShiftId, int companyJobRoleId);
        #endregion

        #region Daily Schedule Shift
        void InsertDailyShiftSchedule(ShiftScheduleDaily entity);
        void UpdateDailyShiftSchedule(ShiftScheduleDaily entity);
        void DeleteDailyShiftSchedule(ShiftScheduleDaily entity);
        IQueryable<ShiftScheduleDaily> GetDailyShiftSchedule(int schedulePeriodId);
        #endregion

        #region Employee View
        IEnumerable<EmployeeSchedule> GetEmployeeScheduleOfPeriod(int schedulePeriod);
        IEnumerable<IEmployeeScheduleDetailModel> PreviewEmployeeScheduleDetail(int schedulePeriodId, int employeeId, int companyShiftId, int roleId);
        void InsertEmployeeSchedule(EmployeeSchedule schedule, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnigns);
        void UpdateEmployeeSchedule(EmployeeSchedule schedule, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnigns);
        void DeleteEmployeeSchedule(int employeeScheduleId);
        void SaveEmployeeSchedule(EmployeeSchedule schedule, out IEnumerable<IScheduleDetailErrorModel> errorsAndWarnigns);
        int GetShiftVacancyInPeriod(int companyShiftId, int schedulerPeriodId, int jobRoleId, int? excludeEmployeeId, bool requiredOnly = false);
        #endregion

        #region Calendar View
        IEnumerable<IShiftViewDay> GetDailyScheduleForShiftView(int schedulePeriodId);
        IEnumerable<EmployeeScheduleDaily> GetEmployeeScheduleDaily(int schedulePeriodId);
        IEnumerable<IScheduleDetailErrorModel> ValidateEmployeeScheduleDaily(EmployeeScheduleDaily entity, int schedulePeriodId = 0);
        EmployeeScheduleDaily UpdateEmployeeScheduleDaily(EmployeeScheduleDaily entity);
        string UpdateEmployeeBreakTimePosition(int employeeScheduleId, DateTime scheduleDate, int breakIndex, int breakTimePosition);
        void DeleteEmployeeScheduleDaily(int employeeScheduleId, DateTime scheduleDate);
        int AddAdhocEmployeeSchedule(int schedulePeriodId, int employeeId, DateTime scheduleDate);
        #endregion

        #region Vacancy View
        IEnumerable<IVacancyViewModel> GetScheduleVacancyView(int schedulePeriodId);
        IEnumerable<Employee> GetAvailableEmployees(int? schedulePeriodId, int? companyShiftId, int? companyJobRoleId);
        void DeleteEmployeeSchedule(int employeeId, int schedulePeriodId, int companyShiftId);
        #endregion

        #region EmployeeSchedule
        IEnumerable<IShiftViewDay> GetEmployeeScheduleBaseline(Guid employeeGuid);
        IEnumerable<EmployeeScheduleDaily> GetEmployeeScheduleOverride(Guid employeeGuid);
        IEnumerable<IShiftViewDay> GetTeamScheduleBaseline(IEnumerable<Guid> employeeGuids);
        IEnumerable<EmployeeScheduleDaily> GetTeamScheduleOverride(IEnumerable<Guid> employeeGuids);
        void UpdateScheduleJobOrderFromScheduleChange(int employeeScheduleId);
        void UpdateJobOrderFromEmployeeSchedules(IEnumerable<EmployeeSchedule> entriesToPublish);
        IList<Employee> GetEmployeeListForScheduleFilter();
        #endregion
    }

    public interface IEmployeeScheduleDetailModel
    {
        DateTime ScheduleDate { get; set; }
        long StartTimeTicks { get; set; }
        long EndTimeTicks { get; set; }
        int CompanyJobRoleId { get; set; }
        string EntryTitle { get; set; }
        IEnumerable<IScheduleDetailErrorModel> ErrorWarnings { get; set; }
    }

    public interface IScheduleDetailErrorModel
    {
        ScheduleWarningLevel LevelOfError { get; set; }
        ScheduleWarningScope Scope { get; set; }
        int? EmployeeId { get; set; }
        string ErrorMessage { get; set; }
    }

    public interface IShiftViewDay
    {
        ShiftScheduleDaily Shift { get; set; }
        IEnumerable<IEmployeeScheduleViewModel> EmployeeSchedules { get; set; }
    }

    public interface IVacancyViewModel
    {
        ShiftSchedule Shift { get; set; }
        IEnumerable<IRoleScheduleViewModel> RoleVacancy { get; set; }
        IEnumerable<DateTime> ScheduledDates { get; set; }
    }

    public interface IRoleScheduleViewModel
    {
        CompanyJobRole JobRole { get; set; }
        IEnumerable<Employee> ScheduledEmployees { get; set; }
        int Vacancy { get; set; }
        IEnumerable<KeyValuePair<int, decimal>> PlannedHours { get; set; }
        decimal UnplannedHours { get; set; } 
        bool Published { get; set; }
    }

    public interface IEmployeeScheduleViewModel
    {
        int EmployeeScheduleId { get; set; }
        Employee Employee { get; set; }
        int JobRoleId { get; set; }
        int CompanyShiftId { get; set; }
        bool ForDailyAdhoc { get; set; }
        string Title { get; set; }
    }
}
