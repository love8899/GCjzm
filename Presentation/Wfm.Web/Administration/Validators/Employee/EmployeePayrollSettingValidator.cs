using System;
using FluentValidation;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Employee;
using Wfm.Shared.Validators;
using System.Linq;
using Wfm.Admin.Validators.Common;
using Wfm.Admin.Models.Common;


namespace Wfm.Admin.Validators.Employee
{
    public class EmployeePayrollSettingValidator<T> : AbstractValidator<T> where T : EmployeePayrollSettingModel
    {
        public EmployeePayrollSettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id is required");

            RuleFor(x => x.FirstHireDate)
                .NotEmpty().WithMessage("First Hire Date is required");

            RuleFor(x => x.LastHireDate)
                .NotEmpty().WithMessage("Last Hire Date is required")
                .GreaterThanOrEqualTo(x => x.FirstHireDate).When(x => x.LastHireDate.HasValue && x.FirstHireDate.HasValue).WithMessage("Last Hire Date can not be before First Hire Date");

            When(x => x.TerminationDate.HasValue && x.LastHireDate.HasValue, () =>
            {
                RuleFor(x => x.TerminationDate)
                    .GreaterThanOrEqualTo(x => x.LastHireDate).WithMessage("Terminate Date can not be before Last Hire Date");
            });
        }
    }
}
