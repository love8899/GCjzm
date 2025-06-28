using FluentValidation.Attributes;
using Wfm.Admin.Validators.Franchise;
using Wfm.Web.Framework;
using Wfm.Admin.Models.Common;
using System;

namespace Wfm.Admin.Models.Franchises
{
    [Validator(typeof(FranchiseAddressValidator))]
    public partial class FranchiseAddressModel : AddressModel
    {
        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }
        public Guid? FranchiseGuid { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.PrimaryPhone")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.SecondaryPhone")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }


        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        public bool IsHeadOffice { get; set; }

    }
}