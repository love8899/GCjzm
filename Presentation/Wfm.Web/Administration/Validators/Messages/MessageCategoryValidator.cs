using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Messages;

namespace Wfm.Admin.Validators.Messages
{
    public class MessageCategoryValidator : AbstractValidator<MessageCategoryModel>
    {
        public MessageCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CategoryName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.MessageCategory.Fields.CategoryName.Required"));
        }
    }
}
