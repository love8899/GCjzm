using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Franchises
{
    public partial class FranchiseModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Web.Franchises.Franchise.Fields.FranchiseName")]
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Web.Franchises.Franchise.Fields.BusinessNumber")]
        public string BusinessNumber { get; set; }

        [WfmResourceDisplayName("Web.Franchises.Franchise.Fields.PrimaryContactName")]
        public string PrimaryContactName { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.WebSite")]
        public string WebSite { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Web.Franchises.Franchise.Fields.ReasonForDisabled")]
        public string ReasonForDisabled { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int OwnerId { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

    }
}