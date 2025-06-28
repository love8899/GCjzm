using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Companies
{
    public partial class CompanyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }

        [WfmResourceDisplayName("Common.WebSite")]
        public string WebSite { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int? OwnerId { get; set; }

        [WfmResourceDisplayName("Web.Companies.Company.Fields.KeyTechnology")]
        public string KeyTechnology { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        public string OwnerName { get; set; }
        public string EnteredByName { get; set; }
    }
}