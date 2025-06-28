using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Logging
{
    public partial class LogModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [WfmResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [WfmResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [WfmResourceDisplayName("Common.IpAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        [WfmResourceDisplayName("Common.Account")]
        public int? AccountId { get; set; }
        [WfmResourceDisplayName("Admin.System.Log.Fields.AccountEmail")]
        public string AccountEmail { get; set; }

        [WfmResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [WfmResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }
    }
}