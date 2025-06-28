using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Employees;

namespace Wfm.Services.Employees
{
    public interface IEmployeeService
    {
        IQueryable<Employee> GetAllEmployeesByCompany(int companyId, bool activeOnly = false, bool includeRegular = true);
        Employee GetEmployeeByCandidateId(int candidateId);
        Employee GetEmployeeByGuid(Guid guid);

        IQueryable<EmployeeAvailability> GetWorktimePreference(int id);
        EmployeeAvailability GetWorktimePreferenceById(int id);
        void AddWorktimePreference(EmployeeAvailability entity);
        void UpdateWorktimePreference(EmployeeAvailability entity);
        void DeleteWorktimePreference(int id);
        bool CheckOverlaping(int employeeId, DayOfWeek dayOfWeek, long startTime, long endTime, int exclusiveId, EmployeeAvailabilityType employeeAvailabilityType,
            DateTime? startDate, DateTime? endDate);

        IEnumerable<EmployeeType> GetAllEmployeeTypes();
        void UpdateEmployee(Candidate candidate, DateTime? hireDate, DateTime? terminationDate, int? companyLocationId, int? primaryJobRoleId);
        void AddNewEmployee(Candidate candidate, int companyId, DateTime? hireDate, int? companyLocationId, int? primaryJobRoleId);
        //
        IQueryable<EmployeeJobRole> GetEmployeJobRoles(int employeeId);
        EmployeeJobRole GetJobRoleById(int employeeJobRoleId);
        void AddEmployeeJobRole(int employeeId, EmployeeJobRole role);
        void UpdateEmployeeJobRole(EmployeeJobRole role);
        void DeleteEmployeeJobRole(int employeeJobRoleId);
    }
}
