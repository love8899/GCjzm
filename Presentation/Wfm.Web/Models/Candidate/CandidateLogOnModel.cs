using System.ComponentModel.DataAnnotations;

namespace Wfm.Web.Models.Candidate
{
    public class CandidateLogOnModel
    {
        [Required(ErrorMessage = "{0} is required.")]
        //[DataType(DataType.EmailAddress)]
        [Display(Name = "User Name")]
        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        //[StringLength(100)]
        //public string Email { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^[^<>]+$", ErrorMessage = "Character < or > not allowed")]
        public string Password { get; set; }
    }
}