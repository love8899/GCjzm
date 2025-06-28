using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Admin.Validators.Accounts;
using Wfm.Shared.Models.Accounts;

namespace Wfm.Admin.Models.Accounts
{
    [Validator(typeof(AccountFullValidator))]
    public partial class AccountFullModel : AccountModel
    {
        [WfmResourceDisplayName("Common.Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }

        [WfmResourceDisplayName("Common.ConfirmNewPassword")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }

        
        //[WfmResourceDisplayName("Admin.Accounts.Account.Fields.HomePhone")]
        //[RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //[MinLength(7)]
        //[MaxLength(15)]
        //public string HomePhone { get; set; }

        //[WfmResourceDisplayName("Admin.Accounts.Account.Fields.MobilePhone")]
        //[RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //[MinLength(7)]
        //[MaxLength(15)]
        //public string MobilePhone { get; set; }

        //[WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        //[RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        //[MinLength(7)]
        //[MaxLength(15)]
        //public string WorkPhone { get; set; }
    }
}

