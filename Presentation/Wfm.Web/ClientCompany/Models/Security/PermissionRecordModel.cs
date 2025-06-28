using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Security
{
    public partial class PermissionRecordModel : BaseWfmModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string Category { get; set; }
    }
}