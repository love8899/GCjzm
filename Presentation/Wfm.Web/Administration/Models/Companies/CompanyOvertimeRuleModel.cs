using FluentValidation.Attributes;
using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Company;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyOvertimeRuleValidator))]
    public class CompanyOvertimeRuleModel : BaseWfmEntityModel
    {

        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.OvertimeRule")]
        public int OvertimeRuleSettingId { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.Code")]
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.TypeId")]
        public string TypeName { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.ApplyAfter")]
        public decimal ApplyAfter { get; set; }
        [WfmResourceDisplayName("Admin.Payroll.OvertimeRuleSetting.Fields.Rate")]
        public decimal Rate { get; set; }

      
        //public virtual OvertimeRuleSettingModel OvertimeRuleSettingModel { get; set; }
    }

}
