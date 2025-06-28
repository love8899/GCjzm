using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.CompanyContact
{
    public partial class CompanyContactViewModel : BaseWfmEntityModel
    {

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string WorkPhone { get; set; }

        public string AccountRoleSystemName { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public string ShiftName { get; set; }
       
        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }
    }
}