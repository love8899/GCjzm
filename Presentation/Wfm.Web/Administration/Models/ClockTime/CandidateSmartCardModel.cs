using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.ClockTime;


namespace Wfm.Admin.Models.ClockTime
{
    /// <summary>
    /// One Candidate may have many Smart Card
    /// Only one is Active.
    /// </summary>
    [Validator(typeof(CandidateSmartCardValidator))]
    public class CandidateSmartCardModel : BaseWfmEntityModel
    {
        public CandidateSmartCardModel()
        {
            this.CandidateSmartCardGuid = Guid.NewGuid();         
        }

        public Guid CandidateSmartCardGuid { get; set; }

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


        // For view only
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        public int FranchiseId { get; set; }

        public Guid? CandidateGuid { get; set; }
        //public virtual CandidateModel CandidateModel { get; set; }

    }
}