using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.ClockTime;
using Wfm.Admin.Models.Candidate;


namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateSmartCardMatchModel       // : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.ActivatedDate")]
        [DataType(DataType.Date)]
        public DateTime? ActivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.DeactivatedDate")]
        [DataType(DataType.Date)]
        public DateTime? DeactivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.ReasonForDeactivation")]
        public string ReasonForDeactivation { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        public int FranchiseId { get; set; }

        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CandidateSmartCardMatchStatusId { get; set; }
    }


    public enum CandidateSmartCardMatchStatus
    {
        Active,
        Inactive,
        Partial,
        Unknown
    }
}
