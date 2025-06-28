using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Incident
{
    public class IncidentReportFileModel : BaseWfmEntityModel
    {
        public int IncidentReportId { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.IncidentCategory.Template.Fields.FileName")]
        public string FileName { get; set; }

        public byte[] FileStream { get; set; }
    }
}