using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Incident
{
    public class IncidentReportFileModel : BaseWfmEntityModel
    {
        public int IncidentReportId { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.IncidentCategory.Template.Fields.FileName")]
        public string FileName { get; set; }

        public byte[] FileStream { get; set; }
    }
}
