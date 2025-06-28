using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;

namespace Wfm.Admin.Validators.JobOrder
{
    public class JobOrderStatusValidator : AbstractValidator<JobOrderStatusModel>
    {
        public JobOrderStatusValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobOrderStatusName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.JobOrderStatus.Fields.JobOrderStatusName.Required"));
        }
    }
}