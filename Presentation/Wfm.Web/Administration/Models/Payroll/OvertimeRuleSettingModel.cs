using FluentValidation.Attributes;
using Wfm.Admin.Validators.Payroll;
using Wfm.Core.Domain.Payroll;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Payroll
{
    [Validator(typeof(OvertimeRuleSettingValidator))]
    public class OvertimeRuleSettingModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Code")]
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.TypeId")]
        public int TypeId { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.ApplyAfter")]
        public decimal ApplyAfter { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.Rate")]
        public decimal Rate { get; set; }
        
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.OvertimeType")]
        public virtual OvertimeType OvertimeType { get; set; }
    }
}
