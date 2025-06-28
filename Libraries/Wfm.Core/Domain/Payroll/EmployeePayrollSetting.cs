using System;
using System.Collections.Generic;

namespace Wfm.Core.Domain.Payroll
{
    public partial class EmployeePayrollSetting
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<System.DateTime> FirstHireDate { get; set; }
        public Nullable<System.DateTime> LastHireDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public bool Tax_Exempt { get; set; }
        public bool CPP_Exempt { get; set; }
        public bool EI_Exempt { get; set; }
        public bool QPIP_Exempt { get; set; }
        public Nullable<int> PayGroupId { get; set; }
        public Nullable<int> TaxProvinceId { get; set; }
        public string PayStubPassword { get; set; }
        public bool AccrueVacation { get; set; }
        public decimal VacationRate { get; set; }
        public virtual PayGroup PayGroup { get; set; }
    }
}
