using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Models.Common;
using Wfm.Web.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Models.Common;


namespace Wfm.Web.Models.Candidate
{

    [Validator(typeof(CandidateUpdateProfileValidator))]
    public partial class CandidateUpdateProfileModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Prompt = "example@example.com")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        public int SalutationId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Common.FullName")]
        public string FullName { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string EmergencyPhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.BirthDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "1990-03-28")]
        public DateTime? BirthDate { get; set; }


        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }


        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.Education")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Education { get; set; }


        //[WfmResourceDisplayName("Common.Shift")]
        //public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.TransportationId")]
        public int TransportationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LicencePlate")]
        [MaxLength(10)]
        public string LicencePlate { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.MajorIntersection1")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Common.MajorIntersection2")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.PreferredWorkLocation")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string PreferredWorkLocation { get; set; }

        public string CandidateInfo { get; set; }

        public void SetInfo()
        {
            CandidateInfo = FirstName + " " + LastName + "(" + EmployeeId + ")";
        }

        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion1")]
        public int? SecurityQuestion1Id { get; set; }

        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion2")]
        public int? SecurityQuestion2Id { get; set; }

        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion1Answer { get; set; }
        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion2Answer { get; set; }

        public virtual SecurityQuestionModel SecurityQuestionModel { get; set; }

        public virtual SalutationModel SalutationModel { get; set; }

        public virtual TransportationModel TransportationModel { get; set; }
       // public virtual ShiftModel ShiftModel { get; set; }
       
        public bool ShowSecurityQuestions { get; set; }

    }
}