using System.Web.Mvc;

namespace Wfm.Web.Framework.HtmlHelpers
{
    public static class SideBarMenuLinkHelper
    {
        public static string SideBarMenuLink(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            var menuOnOff = string.Empty;
            var currentController = helper.ViewContext.RouteData.GetRequiredString("controller");
            var currentAction = helper.ViewContext.RouteData.GetRequiredString("action");
            if (actionName == currentAction && controllerName == currentController)
            {
                menuOnOff = "current-active";
            }
            else
            {
                menuOnOff = "regular";
            }

            return menuOnOff;
        }
    } // end of calss
}
