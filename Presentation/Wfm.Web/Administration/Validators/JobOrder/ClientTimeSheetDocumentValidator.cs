using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;


namespace Wfm.Admin.Validators.JobOrder
{
    public class ClientTimeSheetDocumentValidator : AbstractValidator<ClientTimeSheetDocumentModel>
    {
        public ClientTimeSheetDocumentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FileName).NotNull()
                .WithMessage("Client timesheet document file name is required.");
        }
    }
}
