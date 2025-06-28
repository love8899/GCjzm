namespace Wfm.Core.Domain.Seo
{
    /// <summary>
    /// Represents a page title SEO adjustment
    /// </summary>
    public enum PageTitleSeoAdjustment
    {
        /// <summary>
        /// Pagename comes after franchisename
        /// </summary>
        PagenameAfterFranchisename = 0,
        /// <summary>
        /// Storename comes after pagename
        /// </summary>
        FranchisenameAfterPagename = 10
    }
}
