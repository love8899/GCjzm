using FluentValidation.Attributes;
using Wfm.Admin.Validators.Employee;


namespace Wfm.Admin.Models.Employee
{
    public partial class EmployeeTD1OverviewModel
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int Year { get; set; }
        public string Province_Code { get; set; }
        public decimal Basic_Amount { get; set; }
        public decimal? TotalCredit { get; set; }
    }
}
