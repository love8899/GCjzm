using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Wfm.Web.Framework.HtmlHelpers
{
    public static class RequiredFieldIndicatorHelper
    {
        public static MvcHtmlString RequiredFieldIndicator(this HtmlHelper helper )
        {
            var output = new StringBuilder();

            output.Append(@"<span class=""required"">");
            
            //output.Append(" <sup> * </sup>");
            output.Append("*");

            output.Append("</span>");

            return new MvcHtmlString(output.ToString());
        }

    } // end of calss

}