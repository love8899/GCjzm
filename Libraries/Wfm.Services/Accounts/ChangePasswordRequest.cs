using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Accounts
{
    public class ChangePasswordRequest
    {
        public Account UserAccount { get; set; }

        public bool ValidateOldPassword { get; set; }
        public PasswordFormat NewPasswordFormat { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string OldPassword { get; set; }

        public int RequestEnteredBy { get; set; }
    }}
