using System;
using System.ComponentModel.DataAnnotations;

namespace Wfm.Web.Framework.CustomAttribute 
{
    public class DisallowHTMLAttribute : RegularExpressionAttribute
    {

        public DisallowHTMLAttribute()
            : base(@"^[^<>]+$")
        {
        }

        public override string FormatErrorMessage(string name)
        {
           // : base(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>")

            return String.Format("The field {0} cannot contain html tags", name);

        }


    } // end of class



}