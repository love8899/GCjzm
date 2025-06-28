using FluentValidation;
using Wfm.Admin.Models.Media;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Media
{

    public class DocumentTypeValidator : AbstractValidator<DocumentTypeModel>
    {
        public DocumentTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TypeName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.TypeNameIsRequired"));
            RuleFor(x => x.InternalCode)
         .NotNull();
        }
    }
}