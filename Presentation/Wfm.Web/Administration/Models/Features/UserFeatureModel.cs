using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Features
{
    public class UserFeatureModel : BaseWfmEntityModel
    {
        public string Area {get; set; }

        public int UserId { get; set; }

        public int FeatureId { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public string FeatureDescription { get; set; }

        public bool IsActive { get; set; }
    }

}
