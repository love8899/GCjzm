using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Forums
{
    /// <summary>
    /// Represents a private message
    /// </summary>
    public partial class PrivateMessage : BaseEntity
    {
        /// <summary>
        /// Gets or sets the franchise identifier
        /// </summary>
        public int FranchiseId { get; set; }

        /// <summary>
        /// Gets or sets the account identifier who sent the message
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// Gets or sets the account identifier who should receive the message
        /// </summary>
        public int ToAccountId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by author
        /// </summary>
        public bool IsDeletedByAuthor { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by recipient
        /// </summary>
        public bool IsDeletedByRecipient { get; set; }




        /// <summary>
        /// Gets the account who sent the message
        /// </summary>
        public virtual Account FromAccount { get; set; }

        /// <summary>
        /// Gets the account who should receive the message
        /// </summary>
        public virtual Account ToAccount { get; set; }
    }
}
