using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Infrastructure;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Employees;

namespace Wfm.Shared.Validators
{
    public class EmployeeJobRoleValidator : AbstractValidator<EmployeeJobRoleModel>
    {
        public EmployeeJobRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyJobRoleId)
             .GreaterThan(0)
             .WithMessage(localizationService.GetResource("Web.EmployeeJobRole.CompanyJobRole.Required"));

            RuleFor(x => x.CompanyJobRoleId)
             .Must((m, id) =>
             {
                 var svc = EngineContext.Current.Resolve<IEmployeeService>();
                 return !svc.GetEmployeJobRoles(m.EmployeeIntId).Any(x => x.Id != m.Id && x.CompanyJobRoleId == id &&
                    ((m.ExpiryDate.HasValue && x.EffectiveDate < m.ExpiryDate && m.EffectiveDate < x.ExpiryDate) ||
                    (x.ExpiryDate.HasValue && x.ExpiryDate > m.EffectiveDate) ||
                    (!x.ExpiryDate.HasValue && !m.ExpiryDate.HasValue)));
             })
             .WithMessage(localizationService.GetResource("Web.EmployeeJobRole.CompanyJobRole.NotDuplicate"));

            RuleFor(x => x.EffectiveDate)
                .NotNull()
                .WithMessage("Start date is requred");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.EffectiveDate)
                .WithMessage("End date must be greater than start date");

            RuleFor(x => x.IsPrimary)
             .Must((m, value) =>
             {
                 var svc = EngineContext.Current.Resolve<IEmployeeService>();
                 return !value || !svc.GetEmployeJobRoles(m.EmployeeIntId).Any(x => x.Id != m.Id && x.IsPrimary && 
                    ((m.ExpiryDate.HasValue && x.EffectiveDate < m.ExpiryDate && m.EffectiveDate < x.ExpiryDate) ||
                    (x.ExpiryDate.HasValue && x.ExpiryDate > m.EffectiveDate) ||
                    (!x.ExpiryDate.HasValue && !m.ExpiryDate.HasValue)));
             })
             .WithMessage(localizationService.GetResource("Web.EmployeeJobRole.IsPrimary.NotDuplicate"));
        }
    }
}
