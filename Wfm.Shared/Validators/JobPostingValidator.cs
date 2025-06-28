using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.JobPosting;

namespace Wfm.Shared.Validators.JobPost
{
    public class JobPostingValidator : AbstractValidator<JobPostingEditModel>
    {
        public JobPostingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobTitle).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobTitle.Required"));

            RuleFor(x => x.CompanyId).NotEmpty()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.CompanyLocationId).NotEmpty()
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));

            RuleFor(x => x.PositionId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobPosting.Fields.PositionId.Required"));

            RuleFor(x => x.ShiftId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.ShiftId.Required"));

            //RuleFor(x => x.OpeningNumber).NotEmpty()
            //    .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.OpeningNumber.Required"));

            RuleFor(x => x.JobPostingStatusId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderStatusId.Required"));

            RuleFor(x => x.JobCategoryId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderCategoryId.Required"));

            RuleFor(x => x.JobTypeId).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId.Required"));

            RuleFor(x => x.JobDescription).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobDescription.Required"));

            RuleFor(x => x.StartDate).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));

            RuleFor(x => x.StartTime).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartTime.Required"));

            RuleFor(x => x.EndTime).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.EndTime.Required"));

           When(x => !x.SundaySwitch && !x.MondaySwitch && !x.TuesdaySwitch && !x.WednesdaySwitch && !x.ThursdaySwitch && !x.FridaySwitch && !x.SaturdaySwitch, () =>
            {
                RuleFor(x => x.SundaySwitch).Equal(true)
                    .WithMessage(localizationService.GetResource("JobOrder.WorkingDays.Required"));
            });
        }
    }
}
