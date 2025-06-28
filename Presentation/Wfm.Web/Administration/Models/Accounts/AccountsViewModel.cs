using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using System;

namespace Wfm.Admin.Models.Accounts
{
   
    public partial class AccountsViewModel : BaseWfmEntityModel
    {
        public Guid AccountGuid { get; set; }
        [WfmResourceDisplayName("Common.UserName")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }


        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.FullName")]
        public string FullName { get { return String.Concat(FirstName, " ", LastName).Trim(); } }
    

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.MobilePhone")]      
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]      
        public string WorkPhone { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }
      

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.AccountRoleSystemName")]
        public string AccountRoleSystemName { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }

    }
}
