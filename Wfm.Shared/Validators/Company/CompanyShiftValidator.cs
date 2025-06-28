using FluentValidation;
using System;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Companies;

namespace Wfm.Shared.Validators
{
    public class CompanyShiftValidator : AbstractValidator<CompanyShiftModel>
    {
        public CompanyShiftValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            RuleFor(x => x.ShiftId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.ShiftId.Required"));
            RuleFor(x => x.CompanyLocationId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));
            RuleFor(x => x.CompanyDepartmentId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Admin.Companies.CompanyDepartment.Fields.DepartmentName.Required"));
            RuleFor(x => x.ExpiryDate)
              .GreaterThan(m => new DateTime?(m.EffectiveDate))
              .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.ExpiryDate.GreaterThan"));

            RuleFor(x => x.SchedulePolicyId)
              .NotNull()
              .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.SchedulePolicyId.Required"));
        }
    }
}
