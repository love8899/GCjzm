using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Wfm.Shared.Validators;

namespace Wfm.Shared.Models.Incident
{
    [Validator(typeof(IncidentReportValidator))]
    public class IncidentReportModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.IncidentDateTime")]
        public DateTime IncidentDateTimeUtc { get; set; }
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.Company")]
        public virtual string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.JobOrder")]
        public int? JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.Position")]
        public string Position { get; set; }
        public int? LocationId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.IncidentCategory")]
        public int IncidentCategoryId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Category")]
        public string IncidentCategoryCode { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.Category")]
        public string IncidentCategoryName { get; set; }
        public int ReportedByAccountId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Incident.Candidate.ReportedBy")]
        public string ReportedByUserName { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public string FullName { get { return String.Concat(CandidateFirstName , " " , CandidateLastName); } }

        public SelectList CandidateList { get; set; }
        public SelectList CompanyList { get; set; }
        public SelectList IncidentCategoryList { get; set; }
        public SelectList LocationList { get; set; }
    }
}
