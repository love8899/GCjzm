using FluentValidation.Attributes;
using System;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{

    [Validator(typeof(CandidateDirectHireStatusHistoryValidator))]
    public partial class CandidateDirectHireStatusHistoryModel : BaseWfmEntityModel
    {

        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.JobOrderStatusHistory.Fields.StatusFrom")]
        public int StatusFrom { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.JobOrderStatusHistory.Fields.StatusTo")]
        public int StatusTo { get; set; }

        [WfmResourceDisplayName("Common.InterviewDate")]
        public DateTime? InterviewDate { get; set; }

        [WfmResourceDisplayName("Common.HiredDate")]
        public DateTime? HiredDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Salary")]
        public decimal? Salary { get; set; } 

        [WfmResourceDisplayName("Common.Note")]
        public string Notes { get; set; }

        public int EnteredBy { get; set; }
        public string EnteredName { get; set; }

        [WfmResourceDisplayName("Admin.DirectHireJobOrder.IssueInvoice")]
        public bool IssueInvoice { get; set; }
        [WfmResourceDisplayName("Admin.DirectHireJobOrder.InvoiceDate")]
        public DateTime? InvoiceDate { get; set; }
         
    }
}