using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseWfmModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string Category { get; set; }
    }
}