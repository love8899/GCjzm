using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wfm.Core.Domain.Payroll
{
    public class Payroll_Calendar : BaseEntity
    {
        public Payroll_Calendar()
        {
            this.Payroll_Batch = new List<Payroll_Batch>();
        }

        [ForeignKey("PayGroup")]
        public int PayGroupId { get; set; }

        public int PayPeriodNumber { get; set; }
        public DateTime PayPeriodStartDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
        public DateTime PayPeriodPayDate { get; set; }
        public Nullable<DateTime> PayPeriodCommitDate { get; set; }
        public int Year { get; set; }

        public virtual PayGroup PayGroup { get; set; }
        public virtual ICollection<Payroll_Batch> Payroll_Batch { get; set; }  
    }
}
