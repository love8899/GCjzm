using System;
using System.Web.Routing;
using Wfm.Core.Plugins;



namespace Wfm.Services.Reports
{
    /// <summary>
    /// Provides an interface for creating payment gateways & methods
    /// </summary>
    public partial interface IReportMethod : IPlugin
    {
        #region Methods


        ///// <summary>
        ///// Gets a route for provider configuration
        ///// </summary>
        ///// <param name="actionName">Action name</param>
        ///// <param name="controllerName">Controller name</param>
        ///// <param name="routeValues">Route values</param>
        //void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetReportInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

        Type GetControllerType();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        ReportMethodType ReportMethodType { get; }
        
        #endregion
    }
}
