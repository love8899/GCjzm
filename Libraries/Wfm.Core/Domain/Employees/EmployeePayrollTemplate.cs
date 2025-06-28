using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.Employees
{
    public class EmployeePayrollTemplate
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PayrollItemId { get; set; }
        public decimal Hours { get; set; }
        public decimal Rate { get; set; }

        //public virtual Employee Employee { get; set; }
        //public virtual Payroll_Item PayrollItem { get; set; }
    }
}
