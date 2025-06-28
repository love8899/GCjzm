using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Scheduling;

namespace Wfm.Shared.Models.Scheduling
{
    public class VacancyViewModel : IVacancyViewModel
    {
        public IEnumerable<IRoleScheduleViewModel> RoleVacancy { get; set; }
        public ShiftSchedule Shift { get; set; }
        public IEnumerable<DateTime> ScheduledDates { get; set; }
    }

    public class RoleScheduleViewModel : IRoleScheduleViewModel
    {
        public CompanyJobRole JobRole
        {
            get; set;
        }

        public IEnumerable<Employee> ScheduledEmployees
        {
            get; set;
        }

        public int Vacancy
        {
            get; set;
        }
        public bool Published { get; set; }
        public IEnumerable<KeyValuePair<int, decimal>> PlannedHours { get; set; }
        public decimal UnplannedHours { get; set; }
    }
    public class EmployeeScheduleViewModel : IEmployeeScheduleViewModel
    {
        public int EmployeeScheduleId { get; set; }
        public int CompanyShiftId
        {
            get; set;
        }

        public Employee Employee
        {
            get; set;
        }

        public int JobRoleId
        {
            get; set;
        }
        public bool ForDailyAdhoc
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
    }
}
