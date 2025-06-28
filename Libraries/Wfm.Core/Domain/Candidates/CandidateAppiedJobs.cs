using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateAppliedJobs:BaseEntity
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
    }
}
