using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Models.Common;
using Wfm.Web.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Models.Common;


namespace Wfm.Web.Models.Candidate
{

    [Validator(typeof(CandidateValidator))]
    public partial class CandidateModel : BaseWfmEntityModel
    {

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }


        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.RePassword")]
        public string RePassword { get; set; }


        [WfmResourceDisplayName("Common.Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Prompt = "example@example.com")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.EthnicTypeId")]
        public int? EthnicTypeId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.VetranTypeId")]
        public int? VetranTypeId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.DisabilityStatusId")]
        public bool DisabilityStatus { get; set; }

        [UIHint("SourceEditor")]
        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.Source")]
        public int SourceId { get; set; }

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
        //[DataType(DataType.PhoneNumber)]
        //[Display(Prompt = "416-123-4567")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //[DataType(DataType.PhoneNumber)]
        //[Display(Prompt = "416-123-4567")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //[DataType(DataType.PhoneNumber)]
        //[Display(Prompt = "416-123-4567")]
        public string EmergencyPhone { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.BirthDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "1990-03-28")]
        public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        [RegularExpression(@"(^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$)?", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.BestTimetoCall")]
        public string BestTimetoCall { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]       
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.Education")]       
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Education { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.Education2")]       
        public string Education2 { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.CanRelocate")]
        public bool CanRelocate { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.DateAvailable")]
        public DateTime? DateAvailable { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.CurrentEmployer")]     
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string CurrentEmployer { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.CurrentPay")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string CurrentPay { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.DesiredPay")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string DesiredPay { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

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

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.Entitled")]
        public bool Entitled { get; set; }

        [WfmResourceDisplayName("Common.Note")]      
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int? OwnerId { get; set; }




        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int PipelineJobOrderId { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public string FranchiseName { get; set; }

        public bool HavingWorkTime { get; set; }

        public string CandidateInfo { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.HavingSmartCard")]
        public bool HavingSmartCard { get; set; }

        public string CandidateSmartCardId { get; set; }

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
        public virtual GenderModel GenderModel { get; set; }
        public virtual VetranTypeModel VetranTypeModel { get; set; }

        public virtual SourceModel SourceModel { get; set; }
        public virtual TransportationModel TransportationModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }

        public virtual CandidateAddressModel CandidateAddressModel { get; set; }

        public virtual CandidateAttachmentModel CandidateAttachmentModel { get; set; }

        public virtual IList<CandidateJobOrderModel> CandidateJobOrderModels { get; set; }
        public virtual IList<CandidateWorkHistoryModel> CandidateWorkHistorieModels { get; set; }
        public virtual IList<CandidateKeySkillModel> CandidateKeySkillModels { get; set; }

    }
}