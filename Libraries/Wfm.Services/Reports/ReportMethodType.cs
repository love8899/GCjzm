namespace   Wfm.Services.Reports
{
    /// <summary>
    /// Represents a Report method type
    /// </summary>
    public enum ReportMethodType : int
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// All payment information is entered on the site
        /// </summary>
        Standard = 10,
        /// <summary>
        /// A customer is redirected to a third-party site in order to complete the payment
        /// </summary>
        Redirection = 15,
        /// <summary>
        /// Button
        /// </summary>
        Button = 20,
    }
}
