using System;
using System.Collections.Generic;

namespace Wfm.Core.Domain.Payroll
{
    public partial class Payroll_Batch
    {
        public int Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public System.DateTime LastUpdateDate { get; set; }
        public int Payroll_CalendarId { get; set; }
        public int Year { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<System.DateTime> PayDate { get; set; }
        public virtual Payroll_Calendar Payroll_Calendar { get; set; }
    }
}
