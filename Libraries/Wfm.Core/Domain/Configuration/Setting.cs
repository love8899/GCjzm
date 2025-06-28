namespace Wfm.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity
    {
        public Setting() { }

        public Setting(string name, string value, int franchiseId = 0)
        {
            this.Name = name;
            this.Value = value;
            this.FranchiseId = franchiseId;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the franchise identifier.
        /// </summary>
        /// <value>
        /// The franchise identifier.
        /// </value>
        public int FranchiseId { get; set; }

        /// <summary>
        /// Gets or sets the entered by.
        /// </summary>
        /// <value>
        /// The entered by.
        /// </value>
        public int EnteredBy { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
