using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc ;
using System.Text;

namespace Wfm.Web.Framework.HtmlHelpers
{

    public static class CheckBoxListHelper
    {

        public static MvcHtmlString AllRolesCheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items)
        {

            var output = new StringBuilder();

            output.Append(@"<div class=""checkboxList"">");


            foreach (var item in items)
            {

                output.Append(@"<input type=""checkbox"" name=""");

                output.Append(name);

                output.Append("\" value=\"");

                output.Append(item.Value);

                output.Append("\"");



                if (item.Selected)

                    output.Append(@" checked=""checked""");



                output.Append(" />");

                output.Append(item.Text);

                output.Append("<br />");
              

            }



            output.Append("</div>");



            return  new MvcHtmlString(output.ToString());


        }
      





    } // end of class


} // end of namespace