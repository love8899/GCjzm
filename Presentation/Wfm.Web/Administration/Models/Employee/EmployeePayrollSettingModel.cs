using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Employee;


namespace Wfm.Admin.Models.Employee
{
    [Validator(typeof(EmployeePayrollSettingValidator<EmployeePayrollSettingModel>))]
    public class EmployeePayrollSettingModel
    {
        public int FranchiseId { get; set; }
        public int EmployeeId { get; set; }

        [UIHint("EmployeeTypeEditor")]
        public int EmployeeTypeId { get; set; }

        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? FirstHireDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? LastHireDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? TerminationDate { get; set; }

        [UIHint("PayGroupEditor")]
        public int? PayGroupId { get; set; }

        public int? TaxProvinceId { get; set; }
        public string PayStubPassword { get; set; }
        public bool AccrueVacation { get; set; }
        public decimal VacationRate { get; set; }

        public bool Tax_Exempt { get; set; }
        public bool CPP_Exempt { get; set; }
        public bool EI_Exempt { get; set; }
        public bool QPIP_Exempt { get; set; }
        public bool WSIB_Exempt { get; set; }
    }
}