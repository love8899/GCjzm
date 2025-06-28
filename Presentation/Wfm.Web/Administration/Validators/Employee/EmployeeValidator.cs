using System;
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Employee;
using Wfm.Shared.Validators;


namespace Wfm.Admin.Validators.Employee
{
    public class EmployeeValidator<T> : AbstractValidator<T> where T : EmployeeModel
    {
        public EmployeeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.EmployeeId.Required"));

            RuleFor(x => x.LastName)
                .NotNull().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.SalutationId)
                .NotNull().WithMessage(localizationService.GetResource("Common.SalutationId.Required"));

            RuleFor(x => x.GenderId)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.GenderId.Required"));

            RuleFor(x => x.SocialInsuranceNumber)
                .SetValidator(new SinValidator<string>()).WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid"));

            When(x => !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber) && x.SocialInsuranceNumber.Trim().StartsWith("9"), () =>
            {
                RuleFor(x => x.SINExpiryDate)
                    .NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.Required"));
            });
          
            When(x => x.IsBanned, () =>
            {
                RuleFor(x => x.IsActive).Equal(false).WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.IsActive.MustBeFalse"));
            });
        }
    }

}
