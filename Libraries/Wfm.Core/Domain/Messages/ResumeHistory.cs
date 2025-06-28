using System;
using Wfm.Core.Domain.Accounts;


namespace Wfm.Core.Domain.Messages
{
    /// <summary>
    /// Represents an resume contact history
    /// </summary>
    public partial class ResumeHistory : BaseEntity
    {
        public int ResumeId { get; set; }

        public int AccountId { get; set; }

        public DateTime ContactedOnUtc { get; set; }

        public string Via { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }


        public virtual Resume Resume { get; set; }
        public virtual Account Account  { get; set; }
    }
}
