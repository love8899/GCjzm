using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;

namespace Wfm.Admin.Validators.JobOrder
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

            RuleFor(x => x.StartTime)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartTime.Required"));

            RuleFor(x => x.EndTime)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.EndTime.Required"));

            RuleFor(x => x.RecruiterId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.RecruiterId.Required"))
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.RecruiterId.Required"));

            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.OwnerId.Required"))
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.OwnerId.Required"));

            RuleFor(x => x.PositionId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.JobPosting.Fields.PositionId.Required"));
         
            When(x => !x.SundaySwitch && !x.MondaySwitch && !x.TuesdaySwitch && !x.WednesdaySwitch && !x.ThursdaySwitch && !x.FridaySwitch && !x.SaturdaySwitch, () =>
            {
                RuleFor(x => x.SundaySwitch).Equal(true)
                    .WithMessage(localizationService.GetResource("JobOrder.WorkingDays.Required"));
            });
        }
    }
}