using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace Wfm.Web.Framework.CustomAttribute
{

    public class PostalCodeAttribute : ValidationAttribute
    {
          bool FoundMatch = false;
          string Pattern = @"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$";

        public override bool IsValid(object value)
        {
            try
            {
                FoundMatch = Regex.IsMatch(value.ToString(), Pattern );
            }
            catch (ArgumentException ex)
            {
                 ex.Message.ToString();
            }
            return FoundMatch;

        }


    }


}