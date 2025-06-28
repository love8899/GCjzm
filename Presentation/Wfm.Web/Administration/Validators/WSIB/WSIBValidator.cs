using FluentValidation;
using Wfm.Admin.Models.WSIBs;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.WSIBS
{
    public class WSIBValidator : AbstractValidator<WSIBModel>
    {
        public WSIBValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.Fields.Code.Required"));
            RuleFor(x => x.ProvinceId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.StateProvinceIsRequired"));
            RuleFor(x=>x.StartDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CalculateCandidateWorkTime.Fields.FromDate.Required"));

            RuleFor(x => x.EndDate)
                .Must((e, x) => { if (x == null) return true; else return x > e.StartDate; })
                .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"));
            RuleFor(x => x.Rate)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.WSIB.Rate.GreateThan"));

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Features.Feature.Fields.Description.Required"));
                
        }
    }
}