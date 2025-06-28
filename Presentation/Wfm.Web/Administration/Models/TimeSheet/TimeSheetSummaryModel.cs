using Wfm.Web.Framework;


namespace Wfm.Admin.Models.TimeSheet
{
    public class TimeSheetSummaryModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }

        [WfmResourceDisplayName("Common.Year")]
        public int Year { get; set; }
        [WfmResourceDisplayName("Common.WeekOfYear")]
        public int WeekOfYear { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.SubmittedHours")]
        public decimal SubmittedHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ApprovedHours")]
        public decimal ApprovedHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.TotalHours")]
        public decimal TotalHours { get; set; }
    }
}
