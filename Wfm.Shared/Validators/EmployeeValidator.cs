using FluentValidation;
using System;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Employees;

namespace Wfm.Shared.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeModel>
    {
        public EmployeeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Password.Required"));
            //RuleFor(x => x.Password).Must((e, pass) => { return pass.Length > e.PasswordPolicyModel.MinLength; }).WithMessage("The length of password must be greater than {0}!", x => x.PasswordPolicyModel.MinLength);
            //RuleFor(x => x.Password).Must((e, pass) => { return pass.Length < e.PasswordPolicyModel.MaxLength; }).WithMessage("The length of password must be less than {0}!", x => x.PasswordPolicyModel.MaxLength);
            //RuleFor(x => x.Password)
            //    .Must((e, pass) =>
            //    {
            //        if (e.PasswordPolicyModel.RequireUpperCase)
            //            return pass.Any(x => char.IsUpper(x));
            //        else
            //            return true;
            //    })
            //    .WithMessage("The password must contain at least one upper case letter!");
            //RuleFor(x => x.Password)
            //    .Must((e, pass) =>
            //    {
            //        if (e.PasswordPolicyModel.RequireLowerCase)
            //            return pass.Any(x => char.IsLower(x));
            //        else
            //            return true;
            //    })
            //    .WithMessage("The password must contain at least one lower case letter!");
            //RuleFor(x => x.Password)
            //    .Must((e, pass) =>
            //    {
            //        if (e.PasswordPolicyModel.RequireNumber)
            //            return pass.Any(x => char.IsNumber(x));
            //        else
            //            return true;
            //    })
            //    .WithMessage("The password must contain at least one number!");
            //RuleFor(x => x.Password)
            //    .Must((e, pass) =>
            //    {
            //        if (e.PasswordPolicyModel.RequireSymbol)
            //            return pass.Any(x => char.IsSymbol(x));
            //        else
            //            return true;
            //    })
            //    .WithMessage("The password must contain at least one symbol!");
            //RuleFor(x => x.Password).Length(candidateSettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Common.Password.LengthValidation"), candidateSettings.PasswordMinLength));
            RuleFor(x => x.RePassword).NotEmpty().WithMessage(localizationService.GetResource("Common.ConfirmNewPassword.Required"));
            RuleFor(x => x.RePassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Required"))
                .EmailAddress().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Invalid"));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.FirstName.IsRequired"));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.EmployeeTypeId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.EmployeeTypeId.Required"));

            RuleFor(x => x.GenderId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.GenderId.Required"));

            RuleFor(x => x.HomePhone)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.HomePhone.Required"));

            RuleFor(x => x.HireDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.HireDate.Required"));

            RuleFor(x => x.SocialInsuranceNumber)
                .NotEmpty()
                .SetValidator(new SinValidator<string>())
                .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid"));
          
            RuleFor(x => x.SINExpiryDate)
              .NotNull().When(x => !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber) && x.SocialInsuranceNumber.Trim().StartsWith("9"))
              .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.Required"));
            
            RuleFor(x => x.WorkPermit)
             .NotEmpty().When(x => x.WorkPermitExpiry != null)
             .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.WorkPermit.Required"));

            RuleFor(x => x.WorkPermitExpiry)
             .NotNull().When(x => !String.IsNullOrWhiteSpace(x.WorkPermit))
             .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.WorkPermitExpiryDate.Required"));

            RuleFor(x => x.SINExpiryDate)
                .Must(sinExpiryDate => sinExpiryDate==null)
                .When(x=>!String.IsNullOrEmpty(x.SocialInsuranceNumber) && !x.SocialInsuranceNumber.StartsWith("9"))
             .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.TempSIN"));
           
            RuleFor(x => x.SINExtensionSubmissionDate)
               .Must(sinExtensionSubmissionDate => sinExtensionSubmissionDate==null)
               .When(x => !String.IsNullOrEmpty(x.SocialInsuranceNumber) && !x.SocialInsuranceNumber.StartsWith("9"))
            .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExtensionSubmissionDate.TempSIN"));

            RuleFor(x => x.BirthDate).NotNull().WithMessage(localizationService.GetResource("Common.BirthDate.Required"));
        }
    }
}
