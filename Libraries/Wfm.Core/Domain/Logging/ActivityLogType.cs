namespace Wfm.Core.Domain.Logging
{
    public class ActivityLogType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the activity log type.
        /// </summary>
        /// <value>
        /// The name of the activity log type.
        /// </value>
        public string ActivityLogTypeName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the entered by.
        /// </summary>
        /// <value>
        /// The entered by.
        /// </value>
        public int EnteredBy { get; set; }

        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        /// <value>
        /// The display order.
        /// </value>
        public int DisplayOrder { get; set; }
    }
}