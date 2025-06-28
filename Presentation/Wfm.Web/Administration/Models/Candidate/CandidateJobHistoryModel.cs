using FluentValidation.Attributes;
using System;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{

    [Validator(typeof(CandidateJobOrderValidator))]
    public partial class CandidateJobHistoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatingValue")]
        public int? RatingValue { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatedBy")]
        public string RatedBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatingComment")]
        public string RatingComment { get; set; }

        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime JobOrderStartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? JobOrderEndDate { get; set; }

        [WfmResourceDisplayName("Common.TotalHours")]
        public decimal? TotalHours { get; set; }
    }

}
