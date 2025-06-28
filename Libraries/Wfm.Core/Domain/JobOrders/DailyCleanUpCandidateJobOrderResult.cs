using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.JobOrders
{
    public class DailyCleanUpCandidateJobOrderResult
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public DateTime? TheDayBeforeLastWorkingDate { get; set; }
        public DateTime? TwoDaysBeforeLastWorkingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
