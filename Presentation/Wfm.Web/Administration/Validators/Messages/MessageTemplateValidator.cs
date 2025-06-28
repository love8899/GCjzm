using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Messages;

namespace Wfm.Admin.Validators.Messages
{
    public class MessageTemplateValidator : AbstractValidator<MessageTemplateModel>
    {

        public MessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TagName)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.TagName.Required"));

            RuleFor(x => x.Subject)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.Subject.Required"));

            RuleFor(x => x.Body)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.Body.Required"));

            RuleFor(x => x.MessageCategoryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.MessageTemplate.Fields.MessageCategoryId.Required"));
        }
    }
}