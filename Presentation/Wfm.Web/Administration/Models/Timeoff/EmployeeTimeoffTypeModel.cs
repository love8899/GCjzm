using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Admin.Validators.Timeoff;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Timeoff
{
    [Validator(typeof(EmployeeTimeoffTypeValidator))]
    public class EmployeeTimeoffTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Admin.EmployeeTimeoffType.Field.Paid")]
        public bool Paid { get; set; }
        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        [WfmResourceDisplayName("Admin.EmployeeTimeoffType.Field.DefaultAnnualEntitlementInHours")]
        public int DefaultAnnualEntitlementInHours { get; set; }
        [WfmResourceDisplayName("Admin.EmployeeTimeoffType.Field.AllowNegative")]
        public bool AllowNegative { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmployeeType")]
        public int EmployeeTypeId { get; set; }
    }
}
