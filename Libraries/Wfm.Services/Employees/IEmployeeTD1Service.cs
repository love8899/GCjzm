using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Employees;


namespace Wfm.Services.Employees
{
    public interface IEmployeeTD1Service
    {
        void InsertEmployeeTD1(EmployeeTD1 PayGroup);

        void UpdateEmployeeTD1(EmployeeTD1 PayGroup);

        void DeleteEmployeeTD1(EmployeeTD1 PayGroup);

        EmployeeTD1 GetEmployeeTD1ById(int id);

        EmployeeTD1 GetEmployeeTD1ByEmployeeAndYearAndProvince(int employeeId, int year, string provinceCode);

        IQueryable<EmployeeTD1> GetAllEmployeeTD1s();

        IQueryable<EmployeeTD1> GetAllEmployeeTD1sByEmployeeId(int employeeId, int? year);
    }
}
