using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    public class RecruiterCompanyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }
        public int AccountId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        //[WfmResourceDisplayName("Admin.Accounts.Account.Fields.MiddleName")]
        //public string MiddleName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.HomePhone")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.MobilePhone")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        public string WorkPhone { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public string FranchiseName { get; set; }
    }
}