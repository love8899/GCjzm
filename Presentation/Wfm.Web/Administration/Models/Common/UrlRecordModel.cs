using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    public partial class UrlRecordModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [WfmResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }
    }
}