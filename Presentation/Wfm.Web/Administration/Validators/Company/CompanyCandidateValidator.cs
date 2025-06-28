using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Companies;


namespace Wfm.Admin.Validators.Company
{
    public class CompanyCandidateValidator : AbstractValidator<CompanyCandidateModel>
    {
        public CompanyCandidateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.CandidateId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyCandidate.Fields.CandidateId.Required"));

            //RuleFor(x => x.StartDate)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Companies.CompanyCandidate.Fields.StartDate.Required"));

            RuleFor(x => x.ReasonForLeave)
                .NotNull().When(x => x.EndDate != null)
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyCandidate.Fields.ReasonForLeave.Required"));

        }
    }
}
 