using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    [Validator(typeof(EmployeeJobRoleValidator))]
    public class EmployeeJobRoleModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.EmployeeId")]
        public int EmployeeIntId { get; set; }
        [WfmResourceDisplayName("Web.EmployeeJobRole.CompanyJobRole")]
        public int CompanyJobRoleId { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime? EffectiveDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? ExpiryDate { get; set; }
        [WfmResourceDisplayName("Web.EmployeeJobRole.IsPrimary")]
        public bool IsPrimary { get; set; }
    }
}
