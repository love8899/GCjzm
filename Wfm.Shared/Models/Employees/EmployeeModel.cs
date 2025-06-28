using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Shared.Models.Policies;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    [Validator(typeof(EmployeeValidator))]
    public class EmployeeModel : BaseWfmEntityModel
    {
        public EmployeeModel()
        {
            IsActive = true;
        }

        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        public string Email2 { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.VetranTypeId")]
        public int? VetranTypeId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string EmergencyPhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Age")]
        public string Age
        {
            get
            {
                return this.BirthDate.HasValue ? string.Format("{0}", (int)((DateTime.Today - this.BirthDate.Value).Days / 365)) : string.Empty;
            }
        }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        [RegularExpression(@"(^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$)?", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SINExtensionSubmissionDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExtensionSubmissionDate { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.WorkPermit")]
        public string WorkPermit { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.WorkPermitExpiry")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? WorkPermitExpiry { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.InactiveReason")]
        public string InactiveReason { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.OnboardingStatus")]
        public string OnboardingStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsEmployee")]
        public bool IsEmployee { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Web.Employee.Fields.PrimaryJobRoleId")]
        public int? PrimaryJobRoleId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education")]
        public string Education { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education2")]
        public string Education2 { get; set; }
        public int? CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CanRelocate")]
        public bool CanRelocate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MajorIntersection1")]
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Common.MajorIntersection2")]
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PreferredWorkLocation")]
        public string PreferredWorkLocation { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        //picture thumbnail
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmployeeType")]
        public string EmployeeType { get; set; }
        public int EmployeeTypeId { get; set; }

        public string EmployeeInfo
        {
            get
            {
                return FirstName + " " + LastName + "(" + EmployeeId + ")";
            }
        }

        public string SalutationName { get; set; }
        public string GenderName { get; set; }
        public string EthnicTypeName { get; set; }
        public string EnteredByUserName { get; set; }
        public string VetranTypeName { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HireDate")]
        public DateTime? HireDate { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TerminationDate")]
        public DateTime? TerminationDate { get; set; }

        public string Intersection
        {
            get
            {
                if (MajorIntersection1 != null && MajorIntersection2 != null)
                    return MajorIntersection1 + " / " + MajorIntersection2;
                else
                    return null;
            }
        }
        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.RePassword")]
        public string RePassword { get; set; }

        //public PasswordPolicyModel PasswordPolicyModel { get; set; }
   }
}
