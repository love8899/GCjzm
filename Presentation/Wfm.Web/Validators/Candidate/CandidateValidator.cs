using System;
using FluentValidation;
using FluentValidation.Validators;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Web.Models.Candidate;
using Wfm.Services.Localization;


namespace Wfm.Web.Validators.Candidate
{
    public class CandidateValidator : AbstractValidator<CandidateModel>
    {
        public CandidateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SourceId).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SourceId.Required"));

            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.UserName.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Email.Invalid"));

            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Password.Required"));
            //RuleFor(x => x.Password).Length(candidateSettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Common.Password.LengthValidation"), candidateSettings.PasswordMinLength));
            RuleFor(x => x.RePassword).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.RePassword.Required"));
            RuleFor(x => x.RePassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));

            RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Common.FirstName.IsRequired"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.SocialInsuranceNumber)
                .SetValidator(new SinValidator<string>())
                .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid"));
            RuleFor(x => x.SINExpiryDate)
                .NotNull().When(x => !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber) && x.SocialInsuranceNumber.Trim().StartsWith("9"))
                .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.Required"));

            RuleFor(x => x.SalutationId).NotEmpty().WithMessage(localizationService.GetResource("Common.SalutationId.Required"));
            //RuleFor(x => x.GenderId).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.GenderId.Required"));

            RuleFor(x => x.HomePhone).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.HomePhone.Required"));
            //RuleFor(x => x.SocialInsuranceNumber).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Required"));

            RuleFor(x => x.ShiftId).NotNull().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.ShiftId.Required"));
            
            RuleFor(x => x.TransportationId).NotNull().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.TransportationId.Required"));
            When(x => x.TransportationId == (int)TransportationEnum.Car, () =>
            {
                RuleFor(x => x.LicencePlate).NotNull().NotEmpty();
            });

            RuleFor(x => x.Entitled).Equal(true).WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Entitled.Required"));

            RuleFor(x => x.SecurityQuestion1Id).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion1.Required"));
            RuleFor(x => x.SecurityQuestion2Id).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion2.Required"));
            RuleFor(x => x.SecurityQuestion1Answer).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion1Answer.Required"));
            RuleFor(x => x.SecurityQuestion2Answer).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion2Answer.Required"));

        }
    }


    public class SinValidator<T> : PropertyValidator
    {

        public SinValidator()
            : base("{PropertyName} is not valid.")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var inputString = context.PropertyValue as string;

            // no input is OK
            if (String.IsNullOrEmpty(inputString))
                return true;
            else
                return CommonHelper.IsValidCanadianSin(inputString);
        }
    
    }

}
