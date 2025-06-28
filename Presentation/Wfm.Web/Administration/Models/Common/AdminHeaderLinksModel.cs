using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    public partial class AdminHeaderLinksModel : BaseWfmModel
    {
        public bool IsAuthenticated { get; set; }
        public string AccountEmailUsername { get; set; }
        
        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}