using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Services.Employees
{
    public interface IEmployeePayrollTemplateService
    {
        void InsertEmployeePayrollTemplate(EmployeePayrollTemplate template);

        void UpdateEmployeePayrollTemplate(EmployeePayrollTemplate template);

        void DeleteEmployeePayrollTemplate(EmployeePayrollTemplate template);

        EmployeePayrollTemplate GetEmployeePayrollTemplateById(int id);

        IQueryable<EmployeePayrollTemplate> GetAllEmployeePayrollTemplates();

        IQueryable<EmployeePayrollTemplate> GetAllEmployeePayrollTemplatesByEmployeeId(int employeeId);

        IList<EmployeePayrollTemplate> PrepareEmployeePayrollTemplates(int franchiseId, int employeeId);

        IList<Payroll_Item> GetPayTemplateItems(int franchiseId);
    }
}
