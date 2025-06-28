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
    public class EmployeeTD1Validator<T> : AbstractValidator<T> where T : EmployeeTD1Model
    {
        public EmployeeTD1Validator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Basic_Amount).GreaterThanOrEqualTo(0m);
            RuleFor(x => x.Child_Amount).GreaterThanOrEqualTo(0m).When(x => x.Child_Amount.HasValue);
            RuleFor(x => x.Age_Amount).GreaterThanOrEqualTo(0m).When(x => x.Age_Amount.HasValue);
            RuleFor(x => x.Pension_Income_Amount).GreaterThanOrEqualTo(0m).When(x => x.Pension_Income_Amount.HasValue);
            RuleFor(x => x.Tuition_Amounts).GreaterThanOrEqualTo(0m).When(x => x.Tuition_Amounts.HasValue);
            RuleFor(x => x.Disablility_Amount).GreaterThanOrEqualTo(0m).When(x => x.Disablility_Amount.HasValue);
            RuleFor(x => x.Spouse_Amount).GreaterThanOrEqualTo(0m).When(x => x.Spouse_Amount.HasValue);
            RuleFor(x => x.Eligible_Dependant_Amount).GreaterThanOrEqualTo(0m).When(x => x.Eligible_Dependant_Amount.HasValue);
            RuleFor(x => x.Caregiver_Amount).GreaterThanOrEqualTo(0m).When(x => x.Caregiver_Amount.HasValue);
            RuleFor(x => x.Infirm_Dependant_Amount).GreaterThanOrEqualTo(0m).When(x => x.Infirm_Dependant_Amount.HasValue);
            RuleFor(x => x.Amount_Transferred_From_Spouse).GreaterThanOrEqualTo(0m).When(x => x.Amount_Transferred_From_Spouse.HasValue);
            RuleFor(x => x.Amount_Transferred_From_Dependant).GreaterThanOrEqualTo(0m).When(x => x.Amount_Transferred_From_Dependant.HasValue);
            RuleFor(x => x.Family_Tax_Benefit).GreaterThanOrEqualTo(0m).When(x => x.Family_Tax_Benefit.HasValue);
            RuleFor(x => x.Senior_Supplementary_Amount).GreaterThanOrEqualTo(0m).When(x => x.Senior_Supplementary_Amount.HasValue);
            RuleFor(x => x.Amount_For_Workers_65_Or_Older).GreaterThanOrEqualTo(0m).When(x => x.Amount_For_Workers_65_Or_Older.HasValue);
            RuleFor(x => x.QC_Deductions).GreaterThanOrEqualTo(0m).When(x => x.QC_Deductions.HasValue);
        }
    }
}
