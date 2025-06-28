using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Models.Common;
using Wfm.Core.Domain.Candidates;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    public partial class CandidateWithAddressModel : BaseWfmEntityModel
    {
        public Guid Guid { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        public string Email2 { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        public int SalutationId { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string EmergencyPhone { get; set; }

        //[WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        //[DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? BirthDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.Age")]
        public string Age { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }    
  
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.DateAvailable")]
        public DateTime? DateAvailable { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TransportationId")]
        public int? TransportationId { get; set; }
      
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

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public bool OnBoarded { get; set; }
        public bool UseForDirectPlacement { get; set; }
    }
}