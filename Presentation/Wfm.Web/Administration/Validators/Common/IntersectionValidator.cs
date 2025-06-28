using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
{
    public class IntersectionValidator : AbstractValidator<IntersectionModel>
    {
        public IntersectionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.IntersectionName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.Intersection.Fields.IntersectionName.Required"));
        }
    }
}
