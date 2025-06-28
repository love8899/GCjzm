using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Client.Models.Candidate;
using Wfm.Client.Models.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Employees
{
    public class EmployeeModel : BaseWfmEntityModel
    {
        public EmployeeModel()
        {
        }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmployeeId")]
        public virtual string EmployeeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public string Email2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SalutationId")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.GenderId")]
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

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MiddleName")]
        [AllowHtml]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.FullName")]
        [AllowHtml]
        public string FullName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        [AllowHtml]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        [AllowHtml]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        [AllowHtml]
        public string EmergencyPhone { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Age")]
        [AllowHtml]
        public string Age { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        [RegularExpression(@"(^\d{3}(\s?|[-]?)\d{3}(\s?|[-]?)\d{3}$)?", ErrorMessage = "Invalid Social Insurance Number")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.SINExpiryDate")]
        [DataType(DataType.Date)]
        [Display(Prompt = "2015-01-31")]
        public DateTime? SINExpiryDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BestTimetoCall")]
        public string BestTimetoCall { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.InactiveReason")]
        public string InactiveReason { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BannedReason")]
        public string BannedReason { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.OnboardingStatus")]
        public string OnboardingStatus { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsEmployee")]
        public bool IsEmployee { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.JobTitle")]
        [AllowHtml]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education")]
        [AllowHtml]
        public string Education { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Education2")]
        [AllowHtml]
        public string Education2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CanRelocate")]
        public bool CanRelocate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DateAvailable")]
        public DateTime? DateAvailable { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CurrentEmployer")]
        [AllowHtml]
        public string CurrentEmployer { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.CurrentPay")]
        public string CurrentPay { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DesiredPay")]
        public string DesiredPay { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.ShiftId")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TransportationId")]
        public int TransportationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LicencePlate")]
        [MaxLength(10)]
        public string LicencePlate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MajorIntersection1")]
        [AllowHtml]
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MajorIntersection2")]
        [AllowHtml]
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PreferredWorkLocation")]
        [AllowHtml]
        public string PreferredWorkLocation { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Entitled")]
        public bool Entitled { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Note")]
        [AllowHtml]
        public string Note { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastIpAddress")]
        [AllowHtml]
        public string LastIpAddress { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastLoginDateUtc")]
        public DateTime? LastLoginDateUtc { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastActivityDateUtc")]
        public DateTime? LastActivityDateUtc { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.OwnerId")]
        public int? OwnerId { get; set; }


        //picture thumbnail
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }



        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.AttachmentId")]
        public int? AttachmentId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PipelineJobOrderId")]
        public int PipelineJobOrderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.FranchiseName")]
        public string FranchiseName { get; set; }

        public bool HavingWorkTime { get; set; }

        public string EmployeeInfo { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HavingSmartCard")]
        public bool HavingSmartCard { get; set; }

        public string CandidateSmartCardId { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HavingBankAccount")]
        public bool HavingBankAccount { get; set; }

        public string BankAccountNumber { get; set; }


        public void SetInfo()
        {
            EmployeeInfo = FirstName + " " + LastName + "(" + EmployeeId + ")";
        }
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


        public virtual SalutationModel Salutation { get; set; }
        public virtual GenderModel Gender { get; set; }
        public virtual EthnicTypeModel EthnicType { get; set; }
        public virtual VetranTypeModel VetranType { get; set; }

        public virtual SourceModel SourceModel { get; set; }
        public virtual TransportationModel TransportationModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }

        public virtual CandidateAddressModel CandidateAddressModel { get; set; }
        public virtual CandidateJobOrderModel CandidateJobOrderModel { get; set; }
   }
}
