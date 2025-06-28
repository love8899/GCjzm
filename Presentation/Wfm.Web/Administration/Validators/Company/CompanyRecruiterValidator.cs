using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyRecruiterValidator : AbstractValidator<RecruiterCompanySimpleModel>
    {
        public CompanyRecruiterValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            RuleFor(x => x.FranchiseId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));
            RuleFor(x => x.AccountId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.RecruiterId.Required"));
        }
    }
}