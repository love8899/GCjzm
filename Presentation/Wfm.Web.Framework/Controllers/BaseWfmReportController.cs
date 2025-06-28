using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Services.Reports;

//using Wfm.Services.Reports;

namespace Wfm.Web.Framework.Controllers
{
    public abstract class BaseWfmReportController : Controller
    {
        public abstract IList<string> ValidateReportForm(FormCollection form);
        public abstract ProcessReportRequest GetReportInfo(FormCollection form);
    }
}

