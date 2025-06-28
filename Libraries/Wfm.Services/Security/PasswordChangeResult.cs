using System;
using System.Collections.Generic;
using System.Linq;

namespace Wfm.Services.Security
{
    public class PasswordChangeResult
    {
        public IList<string> Errors { get; set; }

        public PasswordChangeResult() 
        {
            this.Errors = new List<string>();
        }

        public bool IsSuccess
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        public string ErrorsAsString()
        {
            if (this.IsSuccess)
                return null;
            else
                return String.Join(" | ", this.Errors.ToArray());
        }
    }
}
