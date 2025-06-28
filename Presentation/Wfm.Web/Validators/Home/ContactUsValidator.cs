using FluentValidation;
using Wfm.Core.Domain.Accounts;
using System.Text.RegularExpressions;
using Wfm.Services.Localization;
using Wfm.Web.Models.Home;

namespace Wfm.Web.Validators.Home
{
    public class ContactUsValidator : AbstractValidator<ContactUsModel>
    {
        public ContactUsValidator(ILocalizationService localizationService, AccountSettings accountSettings)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.Subject.Required"));
            RuleFor(x => x.Subject).Length(3, 200).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));

            RuleFor(x => x.ContactName).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.ContactName.Required"));
            RuleFor(x => x.ContactName).Length(3, 100).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));

            RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.Company.Required"));
            RuleFor(x => x.Company).Length(3, 100).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));

            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.Phone.Required"));
            RuleFor(x => x.Phone).Length(3, 50).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.Email.Required"));
            RuleFor(x => x.Email).Length(3, 100).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Email.Invalid"));

            RuleFor(x => x.Message).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.Message.Required"));
            RuleFor(x => x.Message).Length(3, 1000).WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.LengthValidation"));

            RuleFor(x => x.RespondBy).NotEmpty().WithMessage(localizationService.GetResource("Web.Home.ContactUs.Fields.RespondBy.Required"));

        }
    }
}