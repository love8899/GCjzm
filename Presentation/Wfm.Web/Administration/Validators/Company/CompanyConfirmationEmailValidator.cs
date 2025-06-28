using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Companies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyConfirmationEmailValidator:AbstractValidator<CompanyConfirmationEmailModel>
    {
        public CompanyConfirmationEmailValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            RuleFor(x => x.LocationId)
                .NotNull().GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));
            RuleFor(x => x.DepartmentId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.DepartmentId.IsRequired"));
            RuleFor(x => x.JobOrderGuid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Fields.JobOrderId.Required"));
            RuleFor(x => x.Start)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));
            RuleFor(x => x.End)
                .GreaterThanOrEqualTo(m => m.Start)
                .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"))
                .When(x => x.End.HasValue);
        }
    }
}