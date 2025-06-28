using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Validators.Candidate;
using Wfm.Shared.Models.Common;
using Wfm.Shared.Models.Policies;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate basic information
    /// </summary>
    [Validator(typeof(CandidateValidator<CandidateModel>))]
    public partial class CandidateModel : BaseWfmEntityModel
    {
        public CandidateModel()
        {
            CandidatePictureModels = new List<CandidatePictureModel>();
            AddPictureModel = new CandidatePictureModel();
        }

        public Guid CandidateGuid { get; set; }

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
        public string Email { get; set; }

        public string Email2 { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        [UIHint("SalutationEditor")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        [UIHint("GenderEditor")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EthnicTypeId")]
        public int? EthnicTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.VetranTypeId")]
        public int? VetranTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DisabilityStatusId")]
        public bool DisabilityStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SourceId")]
        public int? SourceId { get; set; }

        public string WebSite { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Common.FullName")]
        public string FullName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string EmergencyPhone { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("BirthDateEditor")]
        public DateTime? BirthDate { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Age")]
        public string Age { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        //[RegularExpression(@"(^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$)?", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BestTimetoCall")]
        public string BestTimetoCall { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.InactiveReason")]
        public string InactiveReason { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BannedReason")]
        public string BannedReason { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.OnboardingStatus")]
        public string OnboardingStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsEmployee")]
        public bool IsEmployee { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education")]
        public string Education { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education2")]
        public string Education2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CanRelocate")]
        public bool CanRelocate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DateAvailable")]
        public DateTime? DateAvailable { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CurrentEmployer")]
        public string CurrentEmployer { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CurrentPay")]
        public string CurrentPay { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DesiredPay")]
        public string DesiredPay { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        [UIHint("ShiftEditor")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TransportationId")]
        [UIHint("TransportationEditor")]
        public int TransportationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LicencePlate")]
        [MaxLength(10)]
        public string LicencePlate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MajorIntersection1")]      
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Common.MajorIntersection2")]       
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PreferredWorkLocation")]
        public string PreferredWorkLocation { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Entitled")]
        public bool Entitled { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        ///[AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        public string EnteredName { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int? OwnerId { get; set; }


        //picture thumbnail
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }


        [WfmResourceDisplayName("Common.Franchise")]
        public string FranchiseName { get; set; }

        public bool HavingWorkTime { get; set; }

        public string CandidateInfo { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HavingSmartCard")]
        public bool HavingSmartCard { get; set; }

        public string CandidateSmartCardId { get; set; }


        //[WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HavingBankAccount")]
        //public bool HavingBankAccount { get; set; }

        //public string BankAccountNumber { get; set; }
        [WfmResourceDisplayName("Common.UseForDirectPlacement")]
        public bool UseForDirectPlacement { get; set; }


        public void SetInfo()
        {
            CandidateInfo = String.Concat(FirstName , " " , LastName , "(" , EmployeeId , ")");
        }
        public string Intersection
        {
            get
            {
                if (MajorIntersection1 != null && MajorIntersection2 != null)
                    return String.Concat(MajorIntersection1 , " / " , MajorIntersection2);
                else
                    return null;
            }
        }


        public virtual SalutationModel SalutationModel { get; set; }
        public virtual GenderModel GenderModel { get; set; }
        public virtual EthnicTypeModel EthnicTypeModel { get; set; }
        public virtual VetranTypeModel VetranTypeModel { get; set; }

        public virtual SourceModel SourceModel { get; set; }
        public virtual TransportationModel TransportationModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }

        //[NotMapped]
        public virtual AddressModel CandidateAddressModel { get; set; }
        public virtual CandidateJobOrderModel CandidateJobOrderModel { get; set; }

        public virtual IList<CandidateJobOrderModel> CandidateJobOrderModels { get; set; }
        public virtual IList<CandidateWorkHistoryModel> CandidateWorkHistorieModels { get; set; }
        public virtual IList<CandidateKeySkillModel> CandidateKeySkillModels { get; set; }
        public virtual IList<CandidateAddressModel> CandidateAddressModels { get; set; }
        public virtual IList<CandidateAttachmentModel> CandidateAttachmentModels { get; set; }
        public virtual IList<CandidateTestResultModel> CandidateTestResultModels { get; set; }




        //pictures
        public CandidatePictureModel AddPictureModel { get; set; }
        public IList<CandidatePictureModel> CandidatePictureModels { get; set; }

        //Password Policy
        public PasswordPolicyModel PasswordPolicyModel { get; set; }
 
    }
}