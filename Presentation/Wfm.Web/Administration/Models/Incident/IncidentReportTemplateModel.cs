using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Incident
{
    public class IncidentReportTemplateModel : BaseWfmEntityModel
    {
        public int IncidentCategoryId { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.IncidentCategory.Template.Fields.FileName")]
        public string FileName { get; set; }

        public byte[] TemplateStream { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
    }
}
