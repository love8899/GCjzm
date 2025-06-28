using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Media;

namespace Wfm.Admin.Validators.Media
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
