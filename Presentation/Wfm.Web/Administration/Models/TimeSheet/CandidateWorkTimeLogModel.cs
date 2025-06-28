using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    public class CandidateWorkTimeLogModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.TimeSheet")]
        public int CandidateWorkTimeId { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTimeLogModel.Fields.OriginalHours")]
        public decimal OriginalHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTimeLogModel.Fields.NewHours")]
        public decimal NewHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTimeLogModel.Fields.LatestChange")]
        public decimal LatestChange { get { return NewHours - OriginalHours; } }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTimeLogModel.Fields.Reason")]
        public string Reason { get; set; }
    }
}
