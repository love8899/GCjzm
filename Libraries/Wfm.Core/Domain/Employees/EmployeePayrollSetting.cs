using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeePayrollSetting
    {
        [NotMapped]
        public int FranchiseId { get; set; }

        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public DateTime? FirstHireDate { get; set; }
        public DateTime? LastHireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool Tax_Exempt { get; set; }
        public bool CPP_Exempt { get; set; }
        public bool EI_Exempt { get; set; }
        public bool QPIP_Exempt { get; set; }
        public bool WSIB_Exempt { get; set; }
        public int? PayGroupId { get; set; }
        public int? TaxProvinceId { get; set; }
        public string PayStubPassword { get; set; }
        public bool AccrueVacation { get; set; }
        public decimal VacationRate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual PayGroup PayGroup { get; set; }
        public virtual StateProvince TaxProvince { get; set; }
    }
}
