using Wfm.Web.Framework;
using Wfm.Client.Models.Common;

namespace Wfm.Client.Models.Franchises
{
    public partial class FranchiseAddressModel : AddressModel
    {
        [WfmResourceDisplayName("Common.FranchiseId")]
        public string FranchiseId { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.FranchiseAddress.Fields.PrimaryPhone")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.FranchiseAddress.Fields.PrimaryPhoneExtension")]
        public string PrimaryPhoneExtension { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.FranchiseAddress.Fields.SecondaryPhone")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.FranchiseAddress.Fields.SecondaryPhoneExtension")]
        public string SecondaryPhoneExtension { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.FranchiseAddress.Fields.FaxNumber")]
        public string FaxNumber { get; set; }


        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }


        [WfmResourceDisplayName("Common.EnteredBy")]
        public string EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public string DisplayOrder { get; set; }

    }
}