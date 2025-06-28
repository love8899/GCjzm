using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderTypeModel : BaseWfmEntityModel
    {
        public string JobOrderTypeName { get; set; }

        //public string Description { get; set; }

        //public string Note { get; set; }

        public bool IsActive { get; set; }

        public int EnteredBy { get; set; }

    }
}