using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core;

namespace Wfm.Shared.Validators
{

    public class SinValidator<T> : PropertyValidator
    {
        public SinValidator()
            : base("{PropertyName} is not valid.")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var inputString = context.PropertyValue as string;

            if (String.IsNullOrEmpty(inputString))
                return false;

            return CommonHelper.IsValidCanadianSin(inputString);
        }
    }
}
