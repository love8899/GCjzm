using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Accounts
{
    public class AccountDelegateHistoryModel : BaseWfmEntityModel
    {
        public int AccountDelegateId { get; set; }
        public int DelegateAccountId { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.Account.Delegate.Account")]
        public string DelegateAccountName { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.Account.Delegate.LoginDateTime")]
        public DateTime LoginDateTime { get; set; }
    }
}
