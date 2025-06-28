using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;


namespace Wfm.Admin.Validators.TimeSheet
{
    public class MissingHourDocumentValidator : AbstractValidator<MissingHourDocumentModel>
    {
        public MissingHourDocumentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FileName).NotNull()
                .WithMessage("Missing hour document file name is required.");
        }
    }
}
