using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.ScheduleTask;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Task
{
    public class ScheduleTaskValidator : AbstractValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.ScheduleTask.Fields.Name.Required"));

            RuleFor(x => x.Seconds)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.ScheduleTask.Fields.Seconds.Required"));

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.ScheduleTask.Fields.Type.Required"));
        }
    }
}