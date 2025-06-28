using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Common;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Common
{
    public class JobBoardValidator : AbstractValidator<JobBoardModel>
    {
        public JobBoardValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobBoardName).NotNull();
            
        }
    }
}