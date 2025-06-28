using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderStatusModel : BaseWfmEntityModel
    {
        public string JobOrderStatusName { get; set; }

        public bool IsActive { get; set; }

        public int EnteredBy { get; set; }
    }
}