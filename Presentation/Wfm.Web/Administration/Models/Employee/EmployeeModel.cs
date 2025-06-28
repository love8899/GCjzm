using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Employee;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Employee
{
    [Validator(typeof(EmployeeValidator<EmployeeModel>))]
    public class EmployeeModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Common.Vendor")]
        [UIHint("VendorEditor")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        [UIHint("SalutationEditor")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        [UIHint("GenderEditor")]
        public int GenderId { get; set; }

        [UIHint("LanguageEditor")]
        public int? LanguageId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("BirthDateEditor")]
        public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        //[RegularExpression(@"(^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$)?", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExtensionSubmissionDate { get; set; }

        public string WorkPermit { get; set; }

        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? WorkPermitExpiry { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.InactiveReason")]
        public string InactiveReason { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BannedReason")]
        public string BannedReason { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        public string PayrollReminder { get; set; }
    }
}