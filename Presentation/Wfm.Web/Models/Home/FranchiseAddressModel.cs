using Wfm.Web.Framework;
using System;
using Wfm.Web.Models.Common;
namespace Wfm.Web.Models.Home
{
  
    public partial class FranchiseAddressModel : AddressModel
    {
        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }
        public Guid? FranchiseGuid { get; set; }
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhone")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhone")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }
     
        public bool IsHeadOffice { get; set; }

    }
}