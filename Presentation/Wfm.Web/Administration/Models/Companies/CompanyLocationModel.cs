using FluentValidation.Attributes;
using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Company;
using Wfm.Admin.Models.Common;
using System;

namespace Wfm.Admin.Models.Companies
{
    /// <summary>
    /// Company Location
    /// </summary>
    [Validator(typeof(CompanyLocationValidator))]
    public partial class CompanyLocationModel : AddressModel
    {
        public Guid CompanyGuid { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string PrimaryPhone { get; set; }

        //[WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhoneExtension")]
        //[RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //public string PrimaryPhoneExtension { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string SecondaryPhone { get; set; }

        //[WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhoneExtension")]
        //[RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //public string SecondaryPhoneExtension { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        //[WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.MajorIntersection1")]
        //[StringLength(50)]
        //public string MajorIntersection1 { get; set; }

        //[WfmResourceDisplayName("Common.MajorIntersection2")]
        //[StringLength(50)]
        //public string MajorIntersection2 { get; set; }

        //[WfmResourceDisplayName("Common.Note")]
        //public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
        public System.DateTime? LastPunchClockFileUploadDateTimeUtc { get; set; }
        public System.DateTime? LastWorkTimeCalculationDateTimeUtc { get; set; }


        public virtual CompanyModel CompanyModel { get; set; }
    } 
}