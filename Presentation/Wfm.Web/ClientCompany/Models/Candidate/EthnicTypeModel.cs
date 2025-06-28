using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Candidate
{
    public partial class EthnicTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.EthnicType.Fields.EthnicTypeName")]
        public string EthnicTypeName { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

    }
}