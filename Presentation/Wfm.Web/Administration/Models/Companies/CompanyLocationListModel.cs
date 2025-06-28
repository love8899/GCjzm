using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Validators.Company;
using FluentValidation.Attributes;

namespace Wfm.Admin.Models.Companies
{
    /// <summary>
    /// Company Location
    /// </summary>
    [Validator(typeof(CompanyLocationValidator))]
    public partial class CompanyLocationListModel : AddressModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
    } 
}