using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeeScheduleDailyBreakModel : BaseWfmEntityModel
    {
        public int EmployeeScheduleDailyId { get; set; }
        public long BreakTimeOfDayTicks { get; set; }
        public TimeSpan BreakTimeOfDay { get; set; }
        public decimal BreakLengthInMinutes { get; set; }
    }
}
