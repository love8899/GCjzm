using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Validators.Accounts;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : BaseWfmModel
    {
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}