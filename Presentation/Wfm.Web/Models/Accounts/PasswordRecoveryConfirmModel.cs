using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Validators.Accounts;
using Wfm.Shared.Models.Policies;

namespace Wfm.Web.Models.Accounts
{
    [Validator(typeof(PasswordRecoveryConfirmValidator))]
    public partial class PasswordRecoveryConfirmModel : BaseWfmModel
    {
        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [WfmResourceDisplayName("Common.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool SuccessfullyChanged { get; set; }
        public string Result { get; set; }

        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion1")]
        public string SecurityQuestion1 { get; set; }


        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestion2")]
        public string SecurityQuestion2 { get; set; }


        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion1Answer { get; set; }


        [WfmResourceDisplayName("Candidate.Fields.SecurityQuestionAnswer")]
        public string SecurityQuestion2Answer { get; set; }

        public PasswordPolicyModel PasswordPolicyModel { get; set; }
    }
}