using System;
using System.Collections.Generic;
using Wfm.Admin.Models.Accounts;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Security
{
    public partial class PermissionMappingModel : BaseWfmModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<Tuple<int, string>>();
            AvailableAccountRoles = new List<Tuple<int, string, string>>();
        }
        public List<Tuple<int, string>> AvailablePermissions { get; set; }
        public List<Tuple<int, string, string>> AvailableAccountRoles { get; set; }
    }
}