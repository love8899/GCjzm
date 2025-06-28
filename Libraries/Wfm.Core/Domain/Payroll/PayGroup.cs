using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Franchises;
using System.Linq;

namespace Wfm.Core.Domain.Payroll
{
    public partial class PayGroup : BaseEntity
    {
        public PayGroup()
        {
            this.Payroll_Calendar = new List<Payroll_Calendar>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public int PayFrequencyTypeId { get; set; }
        public bool IsDefault { get; set; }
        public int FranchiseId { get; set; }

        [NotMapped]
        public bool HasCommittedPayroll { get; set; }

        [ForeignKey("FranchiseId")]
        public virtual Franchise Vendor { get; set; }

        [ForeignKey("PayFrequencyTypeId")]
        public virtual PayFrequencyType PayFrequencyType { get; set; }
        public virtual ICollection<Payroll_Calendar> Payroll_Calendar { get; set; }
    }
}
