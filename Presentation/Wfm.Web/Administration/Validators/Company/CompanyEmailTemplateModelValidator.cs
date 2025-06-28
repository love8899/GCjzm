using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyEmailTemplateModelValidator : AbstractValidator<CompanyEmailTemplateModel>
    {
        public CompanyEmailTemplateModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            //RuleFor(x => x.CompanyLocationId)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.ScheduleTask.Fields.Type.Required"));

            RuleFor(x => x.Subject)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.Subject.Required"));

            RuleFor(x => x.Body)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.Body.Required"));

        }
    }
}