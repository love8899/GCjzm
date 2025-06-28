using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate basic information
    /// </summary>
    [Validator(typeof(CandidateOnboardingValidator))]
    public partial class CandidateOnboardingModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Common.FullName")]
        public string FullName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        [RegularExpression(@"^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.OnboardingStatus")]
        public string OnboardingStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsEmployee")]
        public bool IsEmployee { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }
    }
}
