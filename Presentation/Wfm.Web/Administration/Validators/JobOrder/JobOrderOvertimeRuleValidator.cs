using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;


namespace Wfm.Admin.Validators.Company
{
    public class JobOrderOvertimeRuleValidator : AbstractValidator<JobOrderOvertimeRuleModel>
    {
        public JobOrderOvertimeRuleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobOrderId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Fields.JobOrderId.Required"));

            RuleFor(x => x.OvertimeRuleSettingId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Fields.OvertimeRuleSettingId.Required"));
        }
    }
}
 