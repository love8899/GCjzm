using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Companies;

namespace Wfm.Shared.Validators
{
    public class CompanyJobRoleValidator : AbstractValidator<CompanyJobRoleModel>
    {
        public CompanyJobRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PositionId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Web.JobRole.Fields.Position.Required"));

            RuleFor(x => x.CompanyId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.Name)
             .NotNull()
             .WithMessage(localizationService.GetResource("Web.JobRole.Fields.Name.Required"));
            RuleFor(x => x.LocationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.CompanyLocationId.Required"));
        }
    }
}
