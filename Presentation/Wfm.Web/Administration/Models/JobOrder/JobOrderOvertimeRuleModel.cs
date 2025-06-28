using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Company;
using Wfm.Admin.Models.Payroll;
using Wfm.Web.Framework.Mvc;
using System;


namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(JobOrderOvertimeRuleValidator))]
    public class JobOrderOvertimeRuleModel : BaseWfmEntityModel
    {
        public Guid JobOrderGuid { get; set; }
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.OvertimeRule")]
        public int OvertimeRuleSettingId { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        
        public List<SelectListItem> OvertimeRuleSettings { get; set; }

        public virtual OvertimeRuleSettingModel OvertimeRuleSettingModel { get; set; }
    }

}
