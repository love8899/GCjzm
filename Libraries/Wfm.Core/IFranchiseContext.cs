using Wfm.Core.Domain.Franchises;

namespace Wfm.Core
{
    /// <summary>
    /// Franchise context
    /// </summary>
    public interface IFranchiseContext
    {
        /// <summary>
        /// Gets or sets the current franchise
        /// </summary>
        Franchise CurrentFranchise { get; }
    }
}
