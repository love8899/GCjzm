using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;


namespace Wfm.Admin.Validators.JobOrder
{
    public class JobOrderWorkTimeValidator : AbstractValidator<JobOrderWorkTimeModel>
    {
        public JobOrderWorkTimeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ClientTimeSheetDocuments).NotNull()
                .WithMessage("Client timesheet document is required.");
        }
    }
}
