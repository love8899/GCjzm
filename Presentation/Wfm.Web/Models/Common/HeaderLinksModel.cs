using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseWfmModel
    {
        public bool IsAuthenticated { get; set; }
        public string CandidateEmailUsername { get; set; }
        

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}