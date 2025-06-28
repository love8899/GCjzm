using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Models.Policies;
using Wfm.Shared.Validators.Accounts;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(ChangePasswordValidator))]
    public partial class ChangePasswordModel : BaseWfmModel
    {
        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Account.ResetPassword.Fields.OldPassword")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }

        public PasswordPolicyModel PasswordPolicyModel { get; set; }
    }
}