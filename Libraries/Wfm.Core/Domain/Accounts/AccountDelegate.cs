using System;
using System.Collections.Generic;

namespace Wfm.Core.Domain.Accounts
{
    public class AccountDelegate : BaseEntity
    {
        public AccountDelegate()
        {
            this.Histories = new List<AccountDelegateHistory>();
        }
        public int AccountId { get; set; }
        public virtual Account Account { get; set;}
        public int DelegateAccountId { get; set; }
        public virtual Account DelegateAccount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Remark { get; set; }

        public ICollection<AccountDelegateHistory> Histories { get; set; }
    }
}
