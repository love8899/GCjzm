using System;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;




namespace Wfm.Web.Framework.CustomActionResults
{
    public class ExcelResult : ActionResult 
    {
        private DataTable _dt;
        private string _fileName;
        private TableStyle _tableStyle;
        private TableItemStyle _headerStyle;
        private TableItemStyle _itemStyle;




        public ExcelResult(DataTable dt, string fileName,  TableStyle tableStyle, TableItemStyle headerStyle, TableItemStyle itemStyle)
        {
            _dt = dt;
            _fileName = fileName;
          
            _tableStyle = tableStyle;
            _headerStyle = headerStyle;
            _itemStyle = itemStyle;

            // provide defaults
            if (_tableStyle == null)
            {
                _tableStyle = new TableStyle();
                _tableStyle.BorderStyle = BorderStyle.Solid;
                _tableStyle.BorderColor = Color.Black;
                _tableStyle.BorderWidth = Unit.Parse("2px");
            }
            if (_headerStyle == null)
            {
                _headerStyle = new TableItemStyle();
                _headerStyle.BackColor = Color.LightGray;
            }

        }




        public override void ExecuteResult(ControllerContext context)
        {
            // Create HtmlTextWriter
            StringWriter sw = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(sw);

            // Build HTML Table from Items
            if (_tableStyle != null)
                _tableStyle.AddAttributesToRender(tw);
            tw.RenderBeginTag(HtmlTextWriterTag.Table);



            // Create Header Row
            tw.RenderBeginTag(HtmlTextWriterTag.Thead);
            for ( int i = 0; i < _dt.Columns.Count; i++)
            {
                if (_headerStyle != null)
                    _headerStyle.AddAttributesToRender(tw);
                tw.RenderBeginTag(HtmlTextWriterTag.Th);
                tw.Write(_dt.Columns[i].ColumnName);
                tw.RenderEndTag();
            }
            tw.RenderEndTag();



            // Create Data Rows
            tw.RenderBeginTag(HtmlTextWriterTag.Tbody);
            foreach (DataRow dr in _dt.Rows)
            {
                tw.RenderBeginTag(HtmlTextWriterTag.Tr);
                for(int i= 0; i < _dt.Columns.Count; i++)
                {
                    string strValue = dr[i].ToString();
                    strValue = ReplaceSpecialCharacters(strValue);
                    if (_itemStyle != null)
                        _itemStyle.AddAttributesToRender(tw);
                    tw.RenderBeginTag(HtmlTextWriterTag.Td);
                    tw.Write(HttpUtility.HtmlEncode(strValue));
                    tw.RenderEndTag();
                }
                tw.RenderEndTag();
            }
            tw.RenderEndTag(); // tbody

            tw.RenderEndTag(); // table

            WriteFile(_fileName, "application/ms-excel", sw.ToString());
        }


        private static string ReplaceSpecialCharacters(string value)
        {
            value = value.Replace("’", "'");
            value = value.Replace("“", "\"");
            value = value.Replace("”", "\"");
            value = value.Replace("–", "-");
            value = value.Replace("…", "...");
            return value;
        }



        private static void WriteFile(string fileName, string contentType, string content)
        {
            HttpContext context = HttpContext.Current;
            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(content);
            context.Response.End();
        }







    } // End of Class



}