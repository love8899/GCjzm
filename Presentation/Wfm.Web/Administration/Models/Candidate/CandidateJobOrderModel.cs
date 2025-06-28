using System;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.Candidate;
using Wfm.Admin.Models.JobOrder;
using Wfm.Core.Domain.Candidates;
using System.Collections.Generic;


namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateJobOrderValidator))]
    public partial class CandidateJobOrderModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateJobOrderStatusId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatingValue")]
        public int? RatingValue { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatedBy")]
        public string RatedBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatingComment")]
        public string RatingComment { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.HelpfulYesTotal")]
        public int? HelpfulYesTotal { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.HelpfulNoTotal")]
        public int? HelpfulNoTotal { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.TestResult")]
        public string TestResult { get; set; }

        [WfmResourceDisplayName("Common.JobOrderStatus")]
        public string JobOrderStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.TestInfoInCandidate")]
        public string TestInfoInCandidate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut")]
        public DateTime? ClockInOut { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyCandidate.Fields.JobDuration")]
        public decimal? JobDuration { get; set; }

        [WfmResourceDisplayName("Common.TotalHours")]
        public decimal? TotalHours { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyCandidate.Fields.TotalPay")]
        public decimal? TotalPay { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyCandidate.Fields.IsArrivedToday")]
        public bool IsArrivedToday { get; set; }

        public bool HaveSmartCard { get; set; }
        public string JobTitle { get; set; }
        public Guid JobOrderGuid { get; set; }

        //public virtual CandidateModel CandidateModel { get; set; }
        //public virtual JobOrderModel JobOrderModel { get; set; }
        //public virtual CandidateJobOrderStatusModel CandidateJobOrderStatusModel { get; set; }

        //public virtual ICollection<EmployeeTimeChartHistory> TimeSheets { get; set; }
        //public virtual ICollection<PayrollDetailModel> PayDetails { get; set; }
    }

}
