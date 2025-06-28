using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Client.Models.Common;

namespace Wfm.Client.Models.Companies
{
    /// <summary>
    /// Company Location
    /// </summary>
    public partial class CompanyLocationModel : AddressModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhone")]
        [RegularExpression(@"[0-9]{3}\-[0-9]{3}\-[0-9]{4}", ErrorMessage = "eg: 416-456-6789")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhone")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }



        public virtual CompanyModel CompanyModel { get; set; }
    } 
}