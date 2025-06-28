using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Announcements;

namespace Wfm.Admin.Validators
{
    
    public class AnnouncementValidator : AbstractValidator<AnnouncementModel>
    {
        public AnnouncementValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.StartDate.Required"));

            RuleFor(x => x.EndDate)
               .NotNull()
               .WithMessage(localizationService.GetResource("Common.EndDate.Required"));

            RuleFor(x => x.AnnouncementText)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.AnnouncementText.Required"));
            
        }
    }
}
