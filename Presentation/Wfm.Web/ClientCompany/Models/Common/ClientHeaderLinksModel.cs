using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Common
{
    public partial class ClientHeaderLinksModel : BaseWfmModel
    {
        public bool IsAuthenticated { get; set; }
        public string AccountEmailUsername { get; set; }
        
        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}