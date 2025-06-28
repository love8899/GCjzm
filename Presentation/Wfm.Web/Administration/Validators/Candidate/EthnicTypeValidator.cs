using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class EthnicTypeValidator : AbstractValidator<EthnicTypeModel>
    {
        public EthnicTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EthnicTypeName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.TypeNameIsRequired"));
        }
    }
}