using FluentValidation.Attributes;
using Wfm.Shared.Validators.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(AccountRoleValidator))]
    public partial class AccountRoleModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.AccountRoles.Fields.Active")]
        public bool Active { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.AccountRoles.Fields.IsSystemRole")]
        public bool IsClientRole { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.AccountRoles.Fields.SystemName")]
        public string ClientName { get; set; }
    }
}