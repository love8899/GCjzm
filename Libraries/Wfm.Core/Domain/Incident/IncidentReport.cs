using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Incident
{
    /// <summary>
    /// Incident reported by client company
    /// </summary>
    public class IncidentReport : BaseEntity
    {
        public int CandidateId { get; set; }
        public virtual Candidate Candidate { get; set; }
        public DateTime IncidentDateTimeUtc { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int? JobOrderId { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public int? LocationId { get; set; }
        public virtual CompanyLocation Location { get; set; }
        public int IncidentCategoryId { get; set; }
        public IncidentCategory IncidentCategory { get; set; }
        public int ReportedByAccountId { get; set; }
        public Account ReportedByAccount { get; set; }
        public string Note { get; set; }

        public ICollection<IncidentReportFile> ReportFiles { get; set; }
    }
    public enum IncidentReportStatusEnum
    {
        [Description("Reported by Client")]
        ClientReport = 0,
        [Description("Recruiter has Acknowledged")]
        RecruiterAcknowledged,
        [Description("Consider Irrelevant")]
        Removed,
        [Description("Incident Closed")]
        Closed,
        [Description("Incident Expired")]
        Expired,
    }
}
