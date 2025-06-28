using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Accounts
{
    public class AccountChangeSecuirtyQuestionsModel 
    {
     
        [Required(ErrorMessage = "{0} is required.")]
        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion1")]
        public int? SecurityQuestion1Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion2")]
        public int? SecurityQuestion2Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion1Answer { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion2Answer { get; set; }

        public virtual SecurityQuestionModel SecurityQuestionModel { get; set; }
    }
}