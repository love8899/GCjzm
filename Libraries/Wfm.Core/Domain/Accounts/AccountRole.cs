using System.Collections.Generic;
using Wfm.Core.Domain.Security;

namespace Wfm.Core.Domain.Accounts
{
    public class AccountRole : BaseEntity
    {

        private ICollection<PermissionRecord> _permissionRecords;

        public string AccountRoleName { get; set; }
        public string Description { get; set; }
        public string SystemName { get; set; }
        public bool IsClientRole { get; set; }
        public string ClientName { get; set; }
        public bool IsVendorRole { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public string HeaderTitle { get; set; }

        /// <summary>
        /// Gets or sets the permission records
        /// </summary>
        public virtual ICollection<PermissionRecord> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecord>()); }
            protected set { _permissionRecords = value; }
        }
    }
}