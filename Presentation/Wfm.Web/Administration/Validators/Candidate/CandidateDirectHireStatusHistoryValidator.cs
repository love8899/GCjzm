using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateDirectHireStatusHistoryValidator : AbstractValidator<CandidateDirectHireStatusHistoryModel>
    {
        public CandidateDirectHireStatusHistoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.StatusTo)
             .NotNull()
             .GreaterThan(0)
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateJobOrder.Fields.CandidateJobOrderStatusId.Required"));

            RuleFor(x => x.InterviewDate)
                .NotNull()
                .When(x => x.StatusTo == (int)CandidateJobOrderStatusEnum.InterviewScheduled)
                .WithMessage(localizationService.GetResource("Admin.DirectHireJobOrder.InterviewDate.Required"));

            RuleFor(x => x.HiredDate)
              .NotNull()
              .When(x => x.StatusTo == (int)CandidateJobOrderStatusEnum.Hired)
              .WithMessage(localizationService.GetResource("Admin.DirectHireJobOrder.HiredDate.Required"));

            RuleFor(x => x.Salary)
            .NotNull()
            .GreaterThan(0)
            .When(x => x.StatusTo == (int)CandidateJobOrderStatusEnum.Hired)
            .WithMessage(localizationService.GetResource("Admin.DirectHireJobOrder.Salary.Required"));

            RuleFor(x => x.InterviewDate)
               .Null()
               .When(x => x.StatusTo != (int)CandidateJobOrderStatusEnum.InterviewScheduled)
               .WithMessage(localizationService.GetResource("Admin.DirectHireJobOrder.InterviewScheduledDate.Warning"));

            RuleFor(x => x.HiredDate)
              .Null()
              .When(x => x.StatusTo != (int)CandidateJobOrderStatusEnum.Hired)
              .WithMessage(localizationService.GetResource("Admin.DirectHireJobOrder.HiredDate.Warning"));
        }
    }
}


  