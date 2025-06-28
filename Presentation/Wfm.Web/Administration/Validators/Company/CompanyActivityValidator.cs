using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyActivityValidator : AbstractValidator<CompanyActivityModel>
    {
        public CompanyActivityValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                    .NotNull();
            RuleFor(x => x.ActivityTypeId)
                    .NotNull().NotEqual(0)
                    .WithMessage(localizationService.GetResource("Admin.Companies.CompanyActivity.Fields.ActivityTypeId.Required"));
            RuleFor(x => x.ActivityDate)
                    .NotNull();
                    
        }
    }
}