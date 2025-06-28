using System;
using System.Web.Mvc;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;



namespace Wfm.Web.Framework.CustomActionResults
{

    public static class ExcelControllerExtensions 
    {

        public static ActionResult Excel
       (
           this Controller controller,
           DataTable dt,
           string fileName,
           TableStyle tableStyle,
           TableItemStyle headerStyle,
           TableItemStyle itemStyle
       )
        {
            return new ExcelResult(dt, fileName,  tableStyle, headerStyle, itemStyle);
        }



    }



}