using System.Collections.Generic;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Security
{
        public class PermissionRecord : BaseEntity
        {
            private ICollection<AccountRole> _accountRoles;

            public string Name { get; set; }

            public string SystemName { get; set; }

            public string Category { get; set; }

            public virtual ICollection<AccountRole> AccountRoles
            {
                get {return _accountRoles ?? (_accountRoles=new List<AccountRole>()); }
                protected set { _accountRoles = value; }
            }
        }
}
