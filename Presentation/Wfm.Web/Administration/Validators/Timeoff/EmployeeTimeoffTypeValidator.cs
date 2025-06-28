using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Timeoff;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Timeoff
{
    public class EmployeeTimeoffTypeValidator : AbstractValidator<EmployeeTimeoffTypeModel>
    {
        public EmployeeTimeoffTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DefaultAnnualEntitlementInHours)
                .GreaterThanOrEqualTo(0);
        }
    }
}