
namespace Wfm.Core.Domain.Accounts
{
    public class AccountPasswordHistory : BaseEntity
    {
        public int AccountId { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }
        public int EnteredBy { get; set; }
    }
}
