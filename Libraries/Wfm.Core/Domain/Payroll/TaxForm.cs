using System;

namespace Wfm.Core.Domain.Payroll
{
    public class TaxForm
    {
        public int Id { get; set; }
        public string FormType { get; set; }
        public int Year { get; set; }
        public string Province { get; set; }
        public decimal Income { get; set; }
        public decimal Tax { get; set; }
        public DateTime IssueDate { get; set; }
        public string SlipType { get; set; }
        public bool IsSubmitted { get; set; }
    }
}
