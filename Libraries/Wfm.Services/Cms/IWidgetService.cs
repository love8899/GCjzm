using System.Collections.Generic;

namespace Wfm.Services.Cms
{
    /// <summary>
    /// Widget service interface
    /// </summary>
    public partial interface IWidgetService
    {
        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgets();

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone);

        /// <summary>
        /// Load all widgets
        /// </summary>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadAllWidgets();
    }
}
