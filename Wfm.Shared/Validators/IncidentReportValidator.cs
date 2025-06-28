using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Incident;

namespace Wfm.Shared.Validators
{
    public class IncidentReportValidator : AbstractValidator<IncidentReportModel>
    {
        public IncidentReportValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.IncidentCategoryId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.IncidentReport.Fields.IncidentCategoryId.Required"));

            RuleFor(x => x.CandidateId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.IncidentReport.Fields.CandidateId.Required"));

           
        }
    }
}