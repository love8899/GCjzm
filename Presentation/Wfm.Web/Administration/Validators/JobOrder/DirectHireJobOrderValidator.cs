using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.JobOrder;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Admin.Validators.JobOrder
{
    public class DirectHireJobOrderValidator : AbstractValidator<DirectHireJobOrderModel>
    {
        public DirectHireJobOrderValidator(ILocalizationService localizationService) 
        {
            RuleFor(x => x.JobTitle) 
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobTitle.Required"));

            RuleFor(x=>x.CompanyId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
           
            RuleFor(x => x.FranchiseId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));

            RuleFor(x => x.CompanyLocationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));

            RuleFor(x => x.JobOrderStatusId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderStatusId.Required"));

            RuleFor(x => x.JobOrderTypeId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId.Required"));

            RuleFor(x => x.StartDate)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));

            RuleFor(x => x.JobDescription)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobDescription.Required"));

            RuleFor(x => x.SalaryMin)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMin.Required"));

            RuleFor(x => x.SalaryMax)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.SalaryMax.Required"));

            RuleFor(x => x.RecruiterId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.RecruiterId.Required"))
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.RecruiterId.Required"));

            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.OwnerId.Required"))
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.OwnerId.Required"));

            RuleFor(x => x.FeeTypeId)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeeTypeId.Required"))
               .GreaterThan(0)
               .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeeTypeId.Required")); 

            When(x=>x.FeeTypeId==(int)(FeeTypeEnum.Percent),()=>{
                RuleFor(x => x.FeePercent)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeePercent.Required"));
                RuleFor(x => x.FeeMin)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeeMin.Required"));
                RuleFor(x => x.FeeMax)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeeMax.Required"));
            });

            When(x => x.FeeTypeId == (int)(FeeTypeEnum.Fixed), () =>
            {
                RuleFor(x => x.FeeAmount)
                    .GreaterThan(0)
                    .NotNull()
                    .WithMessage(localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Fields.FeeAmount.Required"));
            });

            RuleFor(x => x.JobOrderCategoryId)
              .NotNull()
              .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobOrderCategoryId.Required"));
        }
    }
}