using System.Collections.Generic;
using Wfm.Shared.Models.Accounts;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Security
{
    public partial class PermissionMappingModel : BaseWfmModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableAccountRoles = new List<AccountRoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<AccountRoleModel> AvailableAccountRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}