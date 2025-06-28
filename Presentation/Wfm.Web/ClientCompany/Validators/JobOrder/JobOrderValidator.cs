using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.JobOrder;

namespace Wfm.Client.Validators.JobOrder
{
    public class JobOrderValidator : AbstractValidator<JobOrderModel>
    {
        public JobOrderValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobTitle)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobTitle.Required"));

            RuleFor(x=>x.CompanyId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.CompanyLocationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));

            RuleFor(x => x.BillingRateCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.BillingRateCode.Required"));

            RuleFor(x => x.ShiftId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.ShiftId.Required"));

            RuleFor(x => x.JobOrderStatusId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderStatusId.Required"));

            RuleFor(x => x.JobOrderCategoryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderCategoryId.Required"));

            RuleFor(x => x.JobOrderTypeId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId.Required"));

            RuleFor(x => x.JobDescription)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobDescription.Required"));

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));

            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.EndDate.Required"));

            RuleFor(x => x.StartTime)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartTime.Required"));

            RuleFor(x => x.EndTime)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.EndTime.Required"));
        }
    }
}