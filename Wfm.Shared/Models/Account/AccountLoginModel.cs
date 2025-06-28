using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Shared.Validators.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(AccountLoginValidator))]
    public class AccountLoginModel :BaseWfmModel
    {
        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Common.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}