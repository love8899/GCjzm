using System;
using FluentValidation;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;
using Wfm.Shared.Validators;
using System.Linq;
using Wfm.Admin.Validators.Common;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateValidator<T> : AbstractValidator<T> where T : CandidateModel
    {
        public CandidateValidator(ILocalizationService localizationService)
        {
            When(x => x.Id > 0, () =>
            {
                RuleFor(x => x.EmployeeId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.EmployeeId.Required"));
            });

            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.UserName.Required"));
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Invalid"));

            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Password.Required"));
            RuleFor(x => x.Password).Must((e, pass) => { return pass != null && pass.Length >= e.PasswordPolicyModel.MinLength; }).WithMessage(localizationService.GetResource("Common.PasswordPolicy.MinLength"), x => x.PasswordPolicyModel.MinLength);
            RuleFor(x => x.Password).Must((e, pass) => { return pass != null && pass.Length <= e.PasswordPolicyModel.MaxLength; }).WithMessage(localizationService.GetResource("Common.PasswordPolicy.MaxLength"), x => x.PasswordPolicyModel.MaxLength);
            RuleFor(x => x.Password)
                .Must((e, pass) =>
                {
                    if (e.PasswordPolicyModel.RequireUpperCase)
                        return pass.Any(x => char.IsUpper(x));
                    else
                        return true;
                })
                .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireUpperCase"));
            RuleFor(x => x.Password)
                .Must((e, pass) =>
                {
                    if (e.PasswordPolicyModel.RequireLowerCase)
                        return pass.Any(x => char.IsLower(x));
                    else
                        return true;
                })
                .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireLowerCase"));
            RuleFor(x => x.Password)
                .Must((e, pass) =>
                {
                    if (e.PasswordPolicyModel.RequireNumber)
                        return pass.Any(x => char.IsNumber(x));
                    else
                        return true;
                })
                .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireNumber"));
            RuleFor(x => x.Password)
                .Must((e, pass) =>
                {
                    if (e.PasswordPolicyModel.RequireSymbol)
                        return pass.Any(x => char.IsSymbol(x));
                    else
                        return true;
                })
                .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireSymbol"));

            RuleFor(x => x.RePassword).NotEmpty().WithMessage(localizationService.GetResource("Common.ConfirmNewPassword.Required"));
            RuleFor(x => x.RePassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));

            RuleFor(x => x.LastName)
                .NotNull().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.SalutationId)
                .NotNull().WithMessage(localizationService.GetResource("Common.SalutationId.Required"));

            RuleFor(x => x.GenderId)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.GenderId.Required"));

            RuleFor(x => x.HomePhone)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.HomePhone.Required"));

            When(x => x.IsEmployee || x.OnboardingStatus == CandidateOnboardingStatusEnum.Started.ToString() || !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber), () =>
            {
                RuleFor(x => x.SocialInsuranceNumber)
                    .SetValidator(new SinValidator<string>())
                    .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid"));
            });

            When(x => (x.IsEmployee || x.OnboardingStatus == CandidateOnboardingStatusEnum.Started.ToString()) && !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber) && x.SocialInsuranceNumber.Trim().StartsWith("9"), () =>
            {
                RuleFor(x => x.SINExpiryDate)
                    .NotNull().NotEmpty()
                    .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.Required"));
            });
          
            When(x => x.IsBanned, () =>
            {
                RuleFor(x => x.BannedReason).NotEmpty();
                RuleFor(x => x.IsActive).Equal(false).WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.IsActive.MustBeFalse"));
            });


            RuleFor(x => x.ShiftId)
                 .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.ShiftId.Required"));

            RuleFor(x => x.TransportationId)
                 .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.TransportationId.Required"));

            RuleFor(x => x.Entitled)
                 .Equal(true).WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Entitled.Required"));

            When(x => x.TransportationId == (int)TransportationEnum.Car, () =>
            {
                RuleFor(x => x.LicencePlate).NotEmpty();
            });

            ///

            RuleFor(x => x.CandidateAddressModel).SetValidator(new AddressModelValidator<AddressModel>(localizationService));

        }
    }

}
