using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderCategoryModel : BaseWfmEntityModel
    {

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

        public int EnteredBy { get; set; }

    }
}