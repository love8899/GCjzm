using FluentValidation;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyContactValidator :AbstractValidator<CompanyContactModel>
    {
        public CompanyContactValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.FirstName.IsRequired"));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.WorkPhone)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.WorkPhone.Required"));
            RuleFor(x => x.CompanyId)
                .NotNull().GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            RuleFor(x => x.CompanyLocationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));
            RuleFor(x => x.CompanyDepartmentId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.DepartmentId.IsRequired"));
            RuleFor(x => x.AccountRoleSystemName)
                .NotNull()
                .NotEqual("None")
                .WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.AccountRoleSystemName.Required"));

        }
    }
}