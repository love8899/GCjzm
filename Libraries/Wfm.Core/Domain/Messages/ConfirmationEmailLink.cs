using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Messages
{
    public class ConfirmationEmailLink:BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ConfirmationEmailLinkGuid { get; set; }
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ValidBefore { get; set; }
        public int EnteredBy { get; set; }

        public bool IsUsed { get; set; }
        public bool? AcceptOrDecline { get; set; }
        public string Note { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
    }
}
