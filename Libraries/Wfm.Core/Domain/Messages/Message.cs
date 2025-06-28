using System;

namespace Wfm.Core.Domain.Messages
{
    /// <summary>
    /// Represents an message
    /// </summary>
    public partial class Message
    {
        public int Id { get; set; }

        public int MessageHistoryId { get; set; }

        public int AccountId { get; set; }

        public bool IsCandidate { get; set; }

        public string Recipient { get; set; }

        public bool ByEmail { get; set; }

        public bool ByMessage { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadOnUtc { get; set; }


        public virtual MessageHistory MessageHistory { get; set; }
    }
}
