using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate JobOrder status change history
    /// </summary>
    public partial class CandidateJobOrderStatusHistoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        public Guid JobOrderGuid { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        //[WfmResourceDisplayName("Admin.Candidate.JobOrderStatusHistory.Fields.StatusTo")]
        //public int StatusTo { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.JobOrderStatusHistory.Fields.RatingValue")]
        public int RatingValue { get; set; }

        [WfmResourceDisplayName("Common.Comment")]
        public string RatingComment { get; set; }

        public int EnteredBy { get; set; }
        public string EnteredName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}