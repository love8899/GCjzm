using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Employee;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Employee
{
    public class CandidateWSIBCommonRateValidator<T> : AbstractValidator<T> where T : CandidateWSIBCommonRateModel
    {
        public CandidateWSIBCommonRateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateId).NotNull();
            RuleFor(x => x.ProvinceId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            When(x => x.StartDate != null && x.EndDate != null, () =>
            {
                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be later than Start Date!");
            });
            RuleFor(x => x.Ratio)
                .LessThanOrEqualTo(1.00m)
                .GreaterThan(0.00m);
        }
    }
}