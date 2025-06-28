using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;

namespace Wfm.Admin.Validators.JobOrder
{
    public class JobOrderCategoryValidator : AbstractValidator<JobOrderCategoryModel>
    {
        public JobOrderCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CategoryName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.JobOrderCategory.Fields.CategoryName.Required"));
        }
    }
}
