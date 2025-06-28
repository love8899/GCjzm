using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Web.Models.JobOrder;

namespace Wfm.Web.Validators.JobOrder
{
    public class JobOrderEmailAFriendValidator : AbstractValidator<JobOrderEmailAFriendModel>
    {
        public JobOrderEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessage(localizationService.GetResource("Web.JobOrder.EmailAFriend.Fields.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage(localizationService.GetResource("Web.JobOrder.EmailAFriend.Fields.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }}
}