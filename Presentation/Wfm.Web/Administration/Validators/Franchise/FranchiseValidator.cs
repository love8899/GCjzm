using FluentValidation;
using System.Text.RegularExpressions;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Franchises;

namespace Wfm.Admin.Validators.Franchise
{
    public class FranchiseValidator : AbstractValidator<FranchiseModel>
    {
        public FranchiseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FranchiseName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Franchises.Franchise.Fields.FranchiseName.Required"));

            //RuleFor(x => x.FranchiseName)
            //    .Must(x =>
            //    {
            //        try
            //        {
            //            return Regex.IsMatch(x, @"^\(?\d{3}\)?[\s\-]?\d{3}\-?\d{4}$");
            //        }
            //        catch
            //        {
            //            return false;
            //        }
            //    })
            //    .WithMessage(localizationService.GetResource("Invalid account name"));
        }
    }
}
