using System.ComponentModel.DataAnnotations;

namespace Wfm.Web.Framework.CustomAttribute
{

  public class CheckKeySkillAttribute :  ValidationAttribute {


      public override bool IsValid(object value)
      {
          string _value = (string)value;
          bool _IsValid = false;

          if (_value == "AAA")
          {
              _IsValid = true;
          }

          return _IsValid;
      }
  }

}




//public class CheckKeySkillAttribute : ValidationAttribute, IClientValidatable
//{


//    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//    {
//        // return base.IsValid(value, validationContext);
//        string _value = (string)value;

//        string sErrorMessage = "invalid key skill";

//        if (_value != "AAA")
//        {
//            return new ValidationResult(sErrorMessage);
//        }

//        return ValidationResult.Success;

//    }



//    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//    {
//        ModelClientValidationRule mcvrTwo = new ModelClientValidationRule();
//        mcvrTwo.ValidationType = "checkskill"; // all lowercase
//        mcvrTwo.ErrorMessage = "invalid key skill .";
//        mcvrTwo.ValidationParameters.Add("param", "Invalid Key Skill");
//        return new List<ModelClientValidationRule> { mcvrTwo };
//    }





//}





//public class CheckKeySkillAttribute :  ValidationAttribute {


//   public override bool  IsValid(object value)
//    {
//                // return base.IsValid(value);
//                  string _value = (string)value;
//                 bool _IsValid = false ;

//                 if ( _value == "AAA"){

//                     _IsValid = true;
//                 }

//                 return _IsValid;
//     }



//}
