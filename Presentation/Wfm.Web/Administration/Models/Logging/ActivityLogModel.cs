using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Logging
{
    /// <summary>
    /// ActivityLog Model
    /// </summary>
    public class ActivityLogModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Logging.ActivityLog.Fields.ActivityLogTypeId")]
        public int ActivityLogTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Logging.ActivityLog.Fields.AccountId")]
        public int? AccountId { get; set; }

        [WfmResourceDisplayName("Admin.Logging.ActivityLog.Fields.AccountName")]
        public string AccountName { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int? FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.FranchiseName")]
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Admin.Logging.ActivityLog.Fields.ActivityLogDetail")]
        public string ActivityLogDetail { get; set; }



        public virtual ActivityLogTypeModel ActivityLogTypeModel { get; set; }
        
    }
}