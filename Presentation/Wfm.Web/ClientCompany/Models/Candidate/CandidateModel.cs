using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wfm.Client.Validators.Candidate;
using Wfm.Shared.Models.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Candidate
{
    /// <summary>
    /// Candidate basic information
    /// </summary>
    [Validator(typeof(CandidateValidator))]
    public partial class CandidateModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }


        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.RePassword")]
        public string RePassword { get; set; }

        public string PasswordResetToken { get; set; }
        public DateTime? TokenExpiryDate { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        public string Email2 { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
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
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        public string EmergencyPhone { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        public string SocialInsuranceNumber { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BestTimetoCall")]
        public string BestTimetoCall { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

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
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TransportationId")]
        public int TransportationId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MajorIntersection1")]
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Common.MajorIntersection2")]
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PreferredWorkLocation")]
        public string PreferredWorkLocation { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Entitled")]
        public bool Entitled { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastIpAddress")]
        public string LastIpAddress { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }


        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int? OwnerId { get; set; }



        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.AttachmentId")]
        public int? AttachmentId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TotalPipeline")]
        public int TotalPipeline { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }


        [WfmResourceDisplayName("Common.Franchise")]
        public string FranchiseName { get; set; }

        public bool HavingWorkTime { get; set; }

        public string CandidateInfo { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HavingSmartCard")]
        public bool HavingSmartCard { get; set; }

        public string CandidateSmartCardId { get; set; }

        public void SetInfo()
        {
            CandidateInfo= FirstName + " " + LastName+ "(" +EmployeeId+")";
        }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        public virtual SalutationModel SalutationModel { get; set; }
        public virtual GenderModel GenderModel { get; set; }
        public virtual EthnicTypeModel EthnicTypeModel { get; set; }
        public virtual VetranTypeModel VetranTypeModel { get; set; }

        public virtual SourceModel SourceModel { get; set; }
        public virtual TransportationModel TransportationModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }


        public virtual IList<CandidateJobOrderModel> CandidateJobOrderModels { get; set; }
       // public virtual IList<CandidateWorkHistoryModel> CandidateWorkHistorieModels { get; set; }
        public virtual IList<CandidateKeySkillModel> CandidateKeySkillModels { get; set; }
        public virtual IList<CandidateAddressModel> CandidateAddressModels { get; set; }
        public virtual IList<CandidateAttachmentModel> CandidateAttachmentModels { get; set; }

    }
}