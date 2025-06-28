using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Incident
{
    public class IncidentReportModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Candidate.Incident.Employee")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.FirstName")]
        public string CandidateFirstName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.IncidentDateTime")]
        public DateTime IncidentDateTimeUtc { get; set; }
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Company")]
        public virtual string CompanyName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.JobOrder")]
        public int? JobOrderId { get; set; }

        public string Position { get; set; }
        public int? LocationId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Location")]
        public string LocationName { get; set; }
        public int IncidentCategoryId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Category")]
        public string IncidentCategoryCode { get; set; }
        public int ReportedByAccountId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.ReportedBy")]
        public string ReportedByUserName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Note")]
        public string Note { get; set; }
        public string FullName { get { return CandidateFirstName + " " + CandidateLastName; } }
        public SelectList CandidateList { get; set; }
        public SelectList IncidentCategoryList { get; set; }
        public SelectList LocationList { get; set; }
    }

}
