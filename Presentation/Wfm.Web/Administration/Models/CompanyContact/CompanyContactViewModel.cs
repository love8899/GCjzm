using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.CompanyContact
{
    public partial class CompanyContactViewModel : BaseWfmEntityModel
    {
        public Guid AccountGuid { get; set; }
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }


        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.FullName")]
        public string FullName { get { return String.Concat(FirstName, " ", LastName).Trim(); } }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        public string WorkPhone { get; set; }

        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.ManagerId")]
        public int ManagerId { get; set; }

        [WfmResourceDisplayName("Common.Title")]
        public string Title { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string CompanyLocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string CompanyDepartmentName { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.AccountRoleSystemName")]
        public string AccountRoleSystemName { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public string ShiftName { get; set; } 
    }
}