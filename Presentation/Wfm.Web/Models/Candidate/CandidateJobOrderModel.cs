using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Models.JobOrder;

namespace Wfm.Web.Models.Candidate
{
    public partial class CandidateJobOrderModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateJobOrderStatusId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.RatingValue")]
        public int? RatingValue { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.RatedBy")]
        public int? RatedBy { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.RatingComment")]
        public string RatingComment { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.HelpfulYesTotal")]
        public int? HelpfulYesTotal { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.HelpfulNoTotal")]
        public int? HelpfulNoTotal { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }




        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.TestResult")]
        public string TestResult { get; set; }

        [WfmResourceDisplayName("Common.JobOrderStatus")]
        public string JobOrderStatus { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateJobOrder.Fields.TestInfoInCandidate")]
        public string TestInfoInCandidate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut")]
        public DateTime? ClockInOut { get; set; }

        public bool IsArrivedToday { get; set; }

        public bool HaveSmartCard { get; set; }





        public virtual CandidateModel CandidateModel { get; set; }
        public virtual JobOrderModel JobOrderModel { get; set; }
        public virtual CandidateJobOrderStatusModel CandidateJobOrderStatusModel { get; set; }

    }

}