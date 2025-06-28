using System.ComponentModel.DataAnnotations;

namespace Wfm.Web.Framework.CustomAttribute
{
    public class MustBeTrueAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }

    }

}