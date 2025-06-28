using FluentValidation.Attributes;
using Wfm.Admin.Validators.Employee;


namespace Wfm.Admin.Models.Employee
{
    [Validator(typeof(EmployeeTD1Validator<EmployeeTD1Model>))]
    public partial class EmployeeTD1Model
    {
        public int EmployeeTD1_Id { get; set; }
        public int CandidateId { get; set; }
        public int Year { get; set; }
        public string Province_Code { get; set; }
        public decimal Basic_Amount { get; set; }
        public decimal? Child_Amount { get; set; }
        public decimal? Age_Amount { get; set; }
        public decimal? Pension_Income_Amount { get; set; }
        public decimal? Tuition_Amounts { get; set; }
        public decimal? Disablility_Amount { get; set; }
        public decimal? Spouse_Amount { get; set; }
        public decimal? Eligible_Dependant_Amount { get; set; }
        public decimal? Caregiver_Amount { get; set; }
        public decimal? Infirm_Dependant_Amount { get; set; }
        public decimal? Amount_Transferred_From_Spouse { get; set; }
        public decimal? Amount_Transferred_From_Dependant { get; set; }
        public decimal? Family_Tax_Benefit { get; set; }
        public decimal? Senior_Supplementary_Amount { get; set; }
        public decimal? Amount_For_Workers_65_Or_Older { get; set; }
        public decimal? QC_Deductions { get; set; }
        public decimal? TotalCredit { get; set; }
    }
}
