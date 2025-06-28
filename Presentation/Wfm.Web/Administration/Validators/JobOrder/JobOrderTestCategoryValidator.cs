using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;

namespace Wfm.Admin.Validators.JobOrder
{
    public class JobOrderTestCategoryValidator : AbstractValidator<JobOrderTestCategoryModel>
    {
        public JobOrderTestCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobOrderId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrderTestCategory.Fields.JobOrderId.Required"));

            RuleFor(x => x.TestCategoryId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrderTestCategory.Fields.TestCategoryId.Required"));
        }
    }
}
