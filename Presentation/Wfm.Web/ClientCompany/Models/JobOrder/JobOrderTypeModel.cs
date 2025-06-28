using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.JobOrder
{
    public partial class JobOrderTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.JobOrderType.Fields.JobOrderTypeName")]
        public string JobOrderTypeName { get; set; }

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