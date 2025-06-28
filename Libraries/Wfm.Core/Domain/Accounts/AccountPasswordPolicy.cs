using Wfm.Core.Domain.Policies;

namespace Wfm.Core.Domain.Accounts
{
    public class AccountPasswordPolicy:BaseEntity
    {
        public string AccountType { get; set; }
        public int PasswordPolicyId { get; set; }
        public virtual PasswordPolicy PasswordPolicy { get; set; }
    }
}
