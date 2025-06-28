namespace Wfm.Core.Domain.Common
{
    /// <summary>
    /// Represents a generic attribute
    /// </summary>
    public partial class GenericAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }
        
        /// <summary>
        /// Gets or sets the key group
        /// </summary>
        public string KeyGroup { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the entered by.
        /// </summary>
        /// <value>
        /// The entered by.
        /// </value>
        public int EnteredBy { get; set; }

        /// <summary>
        /// Gets or sets the franchise identifier.
        /// </summary>
        /// <value>
        /// The franchise identifier.
        /// </value>
        public int FranchiseId { get; set; }
        
    }
}
