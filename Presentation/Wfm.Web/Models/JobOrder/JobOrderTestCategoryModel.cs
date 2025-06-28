using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderTestCategoryModel : BaseWfmEntityModel
    {
        public int JobOrderId { get; set; }

        public int TestCategoryId { get; set; }

        public bool IsActive { get; set; }

        public string Note { get; set; }

        public int EnteredBy { get; set; }

        public int DataModelType { get; set; }

    }
}