using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;

namespace Wfm.Admin.Validators.JobOrder
{
    public class JobOrderTypeValidator : AbstractValidator<JobOrderTypeModel>
    {
        public JobOrderTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobOrderTypeName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.TypeNameIsRequired"));
        }
    }
}