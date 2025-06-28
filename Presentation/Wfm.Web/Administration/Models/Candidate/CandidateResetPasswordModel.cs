using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Admin.Validators.Candidate;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateResetPasswordValidator))]
    public class CandidateResetPasswordModel
    {
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateResetPassword.Fields.PasswordResetToken")]
        public string PasswordResetToken { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateResetPassword.Fields.TokenExpiryDate")]
        public DateTime TokenExpiryDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateResetPassword.Fields.NewPassword")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateResetPassword.Fields.ConfirmNewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}