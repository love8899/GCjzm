using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Common;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Common
{
    public class PositionModelValidator:AbstractValidator<PositionModel>
    {
        public PositionModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code)
             .NotNull();
            RuleFor(x => x.Name)
             .NotNull();

            RuleFor(x => x.CompanyId)
             .NotNull()
             .GreaterThan(0)
             .WithMessage(localizationService.GetLocaleStringResourceByName("Web.Home.ContactUs.Fields.Company.Required").ResourceValue);

        }
    }
}