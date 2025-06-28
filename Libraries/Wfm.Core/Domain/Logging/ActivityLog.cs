namespace Wfm.Core.Domain.Logging
{
    public partial class ActivityLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the activity log type identifier.
        /// </summary>
        /// <value>
        /// The activity log type identifier.
        /// </value>
        public int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public int? AccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        /// <value>
        /// The name of the account.
        /// </value>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the franchise identifier.
        /// </summary>
        /// <value>
        /// The franchise identifier.
        /// </value>
        public int? FranchiseId { get; set; }

        /// <summary>
        /// Gets or sets the name of the franchise.
        /// </summary>
        /// <value>
        /// The name of the franchise.
        /// </value>
        public string FranchiseName { get; set; }

        /// <summary>
        /// Gets or sets the activity log detail.
        /// </summary>
        /// <value>
        /// The activity log detail.
        /// </value>
        public string ActivityLogDetail { get; set; }
        
        
        public virtual ActivityLogType ActivityLogType { get; set; }
    }
}