using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public class ChangePasswordRequest
    {
        public Candidate Candidate{ get; set; }
        public bool ValidateOldPassword { get; set; }
        public bool ApplyPasswordPolicy { get; set; }
        public PasswordFormat NewPasswordFormat { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
