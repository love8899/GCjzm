using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Localization;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current account
        /// </summary>
         Account CurrentAccount { get; set; }

         /// <summary>
         /// Gets or sets the current account role.
         /// </summary>
         /// <value>
         /// The current account role.
         /// </value>
         AccountRole CurrentAccountRole { get;}

         /// <summary>
         /// Gets or sets the current franchise (logged-in manager)
         /// </summary>
         Franchise CurrentFranchise { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }
        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in client area
        /// </summary>
        bool IsCompanyAdministrator { get; set; }
        /// <summary>
        /// If the system is accessed as impersonate way, which account is used from the origin
        /// </summary>
        Account OriginalAccountIfImpersonate { get; set; }
    }
}
