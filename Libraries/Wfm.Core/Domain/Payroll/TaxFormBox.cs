using System.Collections.Generic;

namespace Wfm.Core.Domain.Payroll
{
    public class TaxFormBox
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Form { get; set; }
        public string Box { get; set; }
        public string Name { get; set; }
        public bool Earnings { get; set; }
        public bool Benefits { get; set; }
        public bool Deductions { get; set; }
        public bool Taxes { get; set; }
        public bool IsFixedBox { get; set; }
        public string AdditionalCode { get; set; }
        public bool Internals { get; set; }

        public virtual ICollection<Payroll_Item> PayrollItems { get; set; }
    }
}
