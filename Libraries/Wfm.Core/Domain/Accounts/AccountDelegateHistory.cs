using System;

namespace Wfm.Core.Domain.Accounts
{
    public class AccountDelegateHistory : BaseEntity
    {
        public int AccountDelegateId { get; set; }
        public virtual AccountDelegate AccountDelegate { get; set; }
        public int DelegateAccountId { get; set; }
        public virtual Account DelegateAccount { get; set; }
        public DateTime LoginDateTime { get; set; }
    }
}
