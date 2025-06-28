using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Logging
{
    /// <summary>
    /// Candidate Activity Log Model
    /// </summary>
    public class CandidateActivityLogModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Logging.CandidateActivityLog.Fields.ActivityLogTypeId")]
        public int ActivityLogTypeId { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int? CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Logging.CandidateActivityLog.Fields.CandidateName")]
        public string CandidateName { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int? FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.FranchiseName")]
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Admin.Logging.CandidateActivityLog.Fields.ActivityLogDetail")]
        public string ActivityLogDetail { get; set; }



        public virtual ActivityLogTypeModel ActivityLogTypeModel { get; set; }
        
    }
}