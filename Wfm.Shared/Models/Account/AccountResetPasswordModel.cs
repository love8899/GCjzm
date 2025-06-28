using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Validators.Accounts;
using Wfm.Shared.Models.Policies;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(AccountResetPasswordValidator))]
    public class AccountResetPasswordModel : BaseWfmEntityModel
    {
        public Guid AccountGuid { get; set; }
        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Account.ResetPassword.Fields.PasswordResetToken")]
        public string PasswordResetToken { get; set; }

        [WfmResourceDisplayName("Account.ResetPassword.Fields.TokenExpiryDate")]
        public DateTime? TokenExpiryDate { get; set; }

        [WfmResourceDisplayName("Account.ResetPassword.Fields.OldPassword")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [WfmResourceDisplayName("Common.NewPassword")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [WfmResourceDisplayName("Common.ConfirmNewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

        public PasswordPolicyModel PasswordPolicyModel { get; set; }
       
    }
}