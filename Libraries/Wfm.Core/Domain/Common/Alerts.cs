using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Common
{
    public class Alerts : BaseEntity
    {
        public int CandidateId { get; set; }
        public virtual Candidate Candidate { get; set; }
        public int? JobOrderId { get; set; }
        public virtual JobOrder JobOrder {get;set;}
        public string Message { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public bool Acknowledged { get; set; }
    }
}
