﻿namespace Wfm.Core.Domain.Common
{
    /// <summary>
    /// Search term record (for statistics)
    /// </summary>
    public class SearchTerm : BaseEntity
    {
        /// <summary>
        /// Gets or sets the keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the franchise identifier
        /// </summary>
        public int FranchiseId { get; set; }

        /// <summary>
        /// Gets or sets search count
        /// </summary>
        public int Count { get; set; }
    }
}
