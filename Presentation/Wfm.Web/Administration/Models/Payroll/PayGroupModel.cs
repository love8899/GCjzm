using FluentValidation.Attributes;
using Wfm.Admin.Validators.Payroll;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Payroll
{
    [Validator(typeof(PayGroupValidator))]
    public class PayGroupModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Code")]
        public string Code { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Common.PayFrequency")]
        public int PayFrequencyTypeId { get; set; }

        [WfmResourceDisplayName("Common.Vendor")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.Vendor")]
        public string VendorName { get; set; }

        [WfmResourceDisplayName("Common.IsDefault")]
        public bool IsDefault { get; set; }

        // Helper properties
        public bool HasCommittedPayroll { get; set; } // This does not map to any column in table
        public int Year {get; set;}
        //public virtual FranchiseSlimModel Franchise {get; set;}
       //[WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.OvertimeType")]
       // public virtual PayFrequency OvertimeType { get; set; }
    }
}