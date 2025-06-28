using System.Collections.Generic;

namespace Wfm.Services.Common 
{
    public class GenericResult
    {
        public IList<string> Errors { get; set; }

        public GenericResult() 
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
    }
}
