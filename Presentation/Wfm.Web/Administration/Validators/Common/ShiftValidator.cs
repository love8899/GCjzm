using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
{
    public class ShiftValidator : AbstractValidator<ShiftModel>
    {
        public ShiftValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ShiftName)
             .NotEmpty()
             .WithMessage(localizationService.GetResource("Admin.Configuration.Shift.Fields.ShiftName.Required"));

            When(x => x.CompanyId > 0, () =>
            {
                RuleFor(x => x.MinStartTime).NotEmpty();
                RuleFor(x => x.MaxEndTime).NotEmpty();
                    // not true for overnight shift
                    //.GreaterThan(x => x.MinStartTime).WithMessage("'End Time' must be greater than 'Start Time'");
            });
        }
    }
}