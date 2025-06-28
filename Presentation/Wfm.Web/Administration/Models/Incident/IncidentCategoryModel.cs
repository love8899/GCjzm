using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Incident
{
    public class IncidentCategoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.FranchiseId")]
        public int? FranchiseId { get; set; }
        [WfmResourceDisplayName("Common.Vendor")]
        public string Vendor { get; set; }
        public int IncidentCategoryCode { get; set; }
        [WfmResourceDisplayName("Common.Code")]
        public string Code
        {
            get
            {
                return IncidentCategoryCode.ToString();
            }
        }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        //
        public SelectList CategoryCodeList { get; set; }
        public SelectList VendorList { get; set; }
    }
}
