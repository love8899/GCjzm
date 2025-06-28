using FluentValidation.Attributes;
using Wfm.Admin.Validators.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Accounts
{
    [Validator(typeof(AccountPasswordPolicyValidator))]
    public class AccountPasswordPolicyModel : BaseWfmEntityModel
    {
        public string AccountType { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.PasswordPolicy")]
        public int PasswordPolicyId { get; set; }
    }
}