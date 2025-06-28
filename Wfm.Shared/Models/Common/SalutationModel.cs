using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Common
{
    public partial class SalutationModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Salutation.Fields.SalutationName")]
        public string SalutationName { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int EnteredBy { get; set; }

        public int DisplayOrder { get; set; }
    }
}