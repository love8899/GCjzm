using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Accounts
{
    public class AccountPasswordHistoryModel : BaseWfmEntityModel
    {
        public int AccountId { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }
        public int EnteredBy { get; set; }
    }
}
