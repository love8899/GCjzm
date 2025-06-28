using System.Collections.Generic;

namespace Wfm.Services.Accounts
{
    public class AccountRegistrationResult
    {
        public IList<string> Errors { get; set; }

        public AccountRegistrationResult()
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return this.Errors.Count == 0; }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }


    }
}
