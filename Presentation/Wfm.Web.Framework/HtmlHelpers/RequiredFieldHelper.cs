using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Wfm.Web.Framework.HtmlHelpers
{
    public static class RequiredFieldHelper
    {
        //public static MvcHtmlString RequiredFieldIndicator(this HtmlHelper helper )
        //{
        //    var output = new StringBuilder();

        //    output.Append(@"<span class=""required"">");
            
        //    //output.Append(" <sup> * </sup>");
        //    output.Append("*");

        //    output.Append("</span>");

        //    return new MvcHtmlString(output.ToString());
        //}

        //public static MvcHtmlString RequiredFieldValidationMessage(this HtmlHelper helper)
        //{
        //    return new MvcHtmlString(@"<tr><td></td><td></td></tr>");
        //}

        public static MvcHtmlString RequiredFieldValidationMessage(this HtmlHelper helper, MvcHtmlString validationMessage)
        {
            var output = new StringBuilder();

            output.Append(@"<tr><td></td><td>");

            output.Append(validationMessage.ToHtmlString());

            output.Append(@"</td></tr>");

            return new MvcHtmlString(output.ToString());
        }

    } // end of calss

}