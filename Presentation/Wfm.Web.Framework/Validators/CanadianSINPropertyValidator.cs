using System;
using System.Linq;
using FluentValidation.Validators;
using Wfm.Core;


namespace Wfm.Web.Framework.Validators
{
    public class CanadianSINPropertyValidator : PropertyValidator
    {
        public CanadianSINPropertyValidator()
            : base("Canadian Social Insurance Number is not valid")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var inputString = context.PropertyValue as string;
            if (String.IsNullOrWhiteSpace(inputString))
                return false;

            return CommonHelper.IsValidCanadianSin(inputString);
        }
    }
}
