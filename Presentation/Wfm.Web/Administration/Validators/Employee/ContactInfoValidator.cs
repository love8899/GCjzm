using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Employee;


namespace Wfm.Admin.Validators.Employee
{
    public class ContactInfoValidator<T> : AbstractValidator<T> where T : ContactInfoModel
    {
        public ContactInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Invalid"));

            RuleFor(x => x.HomePhone)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.HomePhone.Required"));
        }
    }

}
