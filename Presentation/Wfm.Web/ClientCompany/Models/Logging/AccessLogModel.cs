using System.Web.Mvc;
using Wfm.Shared.Models.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Logging
{
    public class AccessLogModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Logging.AccessLog.Fields.AccountId")]
        public int AccountId { get; set; }

        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Admin.Logging.AccessLog.Fields.AccountName")]
        public string AccountName { get; set; }

        [WfmResourceDisplayName("Common.IpAddress")]
        public string IpAdress { get; set; }

        [WfmResourceDisplayName("Admin.Logging.AccessLog.Fields.UserAgent")]
        public string UserAgent { get; set; }

        [WfmResourceDisplayName("Admin.Logging.AccessLog.Fields.IsSuccessful")]
        public bool IsSuccessful { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }



        public virtual AccountModel AccountModel { get; set; }
    }
}