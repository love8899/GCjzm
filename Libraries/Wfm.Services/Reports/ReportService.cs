using Wfm.Core.Plugins;
using Wfm.Core.Domain.Reports;

namespace Wfm.Services.Reports
{
    /// <summary>
    /// Payment service
    /// </summary>
    public partial class ReportService : IReportService
    {
        #region Fields

        private readonly ReportSettings _reportSettings;
        private readonly IPluginFinder _pluginFinder;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// 

        public ReportService(){
            this._pluginFinder = new PluginFinder();
            this._reportSettings = new ReportSettings();
        }


        public ReportService(ReportSettings reportSettings, IPluginFinder pluginFinder)
        {
            this._reportSettings = reportSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion

        #region Methods

       

        /// <summary>
        /// Load payment provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found payment provider</returns>
        public virtual IReportMethod LoadReportMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IReportMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IReportMethod>();

            return null;
        }

        
        #endregion
    }
}
