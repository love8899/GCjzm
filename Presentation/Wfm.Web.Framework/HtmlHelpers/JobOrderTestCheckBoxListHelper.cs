using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Web.Framework.HtmlHelpers
{
    public static class JobOrderTestCheckBoxListHelper
    {
        public static MvcHtmlString GetJobOrderTestCheckBoxList(this HtmlHelper helper, List<TestCategory> availableTests, List<JobOrderTestCategory> jobOrderTests, bool editMode = false)
        {
            var output = new StringBuilder();

            for (int i = 0; i < availableTests.Count(); i++)
            {
                if (availableTests[i] != null)
                {
                    output.Append(@"<input type=""checkbox"" class=""check-box"" name=""");
                    output.Append(@"RequiredTests");
                    output.Append("\" value=\"");
                    output.Append(availableTests[i].Id);
                    output.Append("\"");

                    foreach (var jobTest in jobOrderTests)
                    {
                        if (availableTests[i].Id == jobTest.TestCategoryId)
                        {
                            output.Append(@" checked=""checked""");
                        }
                    }

                    if (!editMode)
                    {
                        output.Append(@" disabled=""disabled""");
                    }

                    output.Append("/>");
                    output.Append(@"&nbsp;");
                    output.Append(availableTests[i].TestCategoryName);
                    output.Append(@"<br />");
                }
            }

            return new MvcHtmlString(output.ToString());
        }
    }
}