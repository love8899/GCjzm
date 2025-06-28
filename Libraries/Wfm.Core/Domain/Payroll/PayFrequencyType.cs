using System;
using System.Collections.Generic;

namespace Wfm.Core.Domain.Payroll
{
    public partial class PayFrequencyType
    {
        public PayFrequencyType()
        {
            this.PayGroups = new List<PayGroup>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Frequency { get; set; }
        public virtual ICollection<PayGroup> PayGroups { get; set; }
    }
}
