using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Media;

namespace Wfm.Client.Validators.Media
{
    public class AttachmentTypeValidator : AbstractValidator<AttachmentTypeModel>
    {
        public AttachmentTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TypeName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.TypeNameIsRequired"));
        }
    }
}
