using System;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum subscription item
    /// </summary>
    public partial class ForumSubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum subscription identifier
        /// </summary>
        public Guid SubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier
        /// </summary>
        public int TopicId { get; set; }



        /// <summary>
        /// Gets the account
        /// </summary>
        public virtual Account Account { get; set; }
    }
}
