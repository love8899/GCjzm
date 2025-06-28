using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Shared.Validators.Accounts;

namespace Wfm.Shared.Models.Accounts
{
    [Validator(typeof(AccountValidator))]
    public partial class AccountModel : BaseWfmEntityModel
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

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "416-123-4567")]
        public string WorkPhone { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.ManagerId")]
        public string ManagerId { get; set; } // note: in the table it is saved as int?

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.IsSystemAccount")]
        public bool IsSystemAccount { get; set; }

        public string SystemName { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.IsClientAccount")]
        public bool IsClientAccount { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.LastIpAddress")]
        public string LastIpAddress { get; set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public DateTime? LastActivityDateUtc { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.IsLimitedToFranchises")]
        public bool IsLimitedToFranchises { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string CompanyLocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string CompanyDepartmentName { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public string ShiftName { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.AccountRoleSystemName")]
        public string AccountRoleSystemName { get; set; }
        [WfmResourceDisplayName("Common.BadgeId")]
        public string EmployeeId { get; set; } // note: in the table it is saved as int?
    }
}

