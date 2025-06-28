using FluentValidation;
using System;
using System.Linq;
using Wfm.Core.Infrastructure;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Companies;

namespace Wfm.Shared.Validators
{
    public class CompanyShiftJobRoleValidator : AbstractValidator<CompanyShiftJobRoleModel>
    {
        public CompanyShiftJobRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyShiftId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Web.CompanyShift.Fields.ShiftId.Required"));
            RuleFor(x => x.CompanyJobRoleId)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Web.EmployeeJobRole.CompanyJobRole.Required"));
            RuleFor(x => x.MandantoryRequiredCount)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Web.CompanyShiftJobRole.Fields.MandantoryRequiredCount.GreaterThan"));
        }
    }
    public class CompanyShiftJobRoleGridValidator : AbstractValidator<CompanyShiftJobRoleGridModel>
    {
        public CompanyShiftJobRoleGridValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyJobRole)
              .Must((e, r) =>
              {
                  var svc = EngineContext.Current.Resolve<ICompanyService>();
                  return !svc.GetJobRolesOfShift(e.CompanyShiftId).Any(x => x.Id != e.Id && x.CompanyJobRoleId == r.Id);
              })
              .WithMessage(localizationService.GetResource("Web.CompanyShiftJobRole.Fields.CompanyJobRole.NotDuplicate"), (e, r) => { return r.Name; });
            RuleFor(x => x.MandantoryRequiredCount)
              .GreaterThan(0)
              .WithMessage(localizationService.GetResource("Web.CompanyShiftJobRole.Fields.MandantoryRequiredCount.GreaterThan"));
            RuleFor(x => x.ContingencyRequiredCount)
              .Must((e, n) =>
              {
                  return n >= 0;
              })
              .WithMessage(localizationService.GetResource("Web.CompanyShiftJobRole.Fields.ContingencyRequiredCount.GreaterThanOrEqualTo"));
        }
    }
}
