using FluentValidation;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyDepartmentValidator : AbstractValidator<CompanyDepartmentModel>
    {
        public CompanyDepartmentValidator(ILocalizationService localizationService){
            RuleFor(x => x.CompanyId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.DepartmentName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyDepartment.Fields.DepartmentName.Required"));
            RuleFor(x => x.CompanyLocationId)
                .NotNull().GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));
        }
    }
}