using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Policies;

namespace Wfm.Shared.Validators
{
    public class PasswordPolicyValidator : AbstractValidator<PasswordPolicyModel>
    {
        public PasswordPolicyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code).NotNull();

            RuleFor(x => x.MinLength)
               .NotNull();

            RuleFor(x => x.MaxLength)
               .NotNull();


            RuleFor(x => x.PasswordLifeTime)
               .NotNull();

            RuleFor(x => x.PasswordHistory)
               .NotNull();
        }
    }
}