using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Company;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyContactValidator))]
    public class CompanyContactModel : BaseWfmEntityModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountGuid { get; set; }
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }


        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(7)]
        [MaxLength(15)]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(7)]
        [MaxLength(15)]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.WorkPhone")]
        [RegularExpression(@"^[\+0-9\s\-\(\)]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(7)]
        [MaxLength(15)]
        public string WorkPhone { get; set; }

        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        public int? ShiftId { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.AccountRoleSystemName")]
        public string AccountRoleSystemName { get; set; }
    }
}