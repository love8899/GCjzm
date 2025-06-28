using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Scheduling;

namespace Wfm.Shared.Validators
{
    public class WeeklyDemandValidator : AbstractValidator<WeeklyDemandModel>
    {
        public WeeklyDemandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyDepartmentId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.DepartmentId.IsRequired"));

            RuleFor(x => x.CompanyJobRoleId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Client.Scheduling.WeeklyDemand.Fields.CompanyJobRoleId.Required"));

            RuleFor(x => x.ShiftId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.ShiftId.Required"));

            RuleFor(x => x.SupervisorId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Client.Scheduling.WeeklyDemand.Fields.SupervisorId.Required"));
        }
    }
}
