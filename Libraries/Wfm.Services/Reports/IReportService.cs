
namespace Wfm.Services.Reports
{
    /// <summary>
    /// Report service interface
    /// </summary>
    public partial interface IReportService
    {
        
        /// <summary>
        /// Load payment provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found payment provider</returns>
        IReportMethod LoadReportMethodBySystemName(string systemName);

    }
}
