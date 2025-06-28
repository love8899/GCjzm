using System;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchJobPostingModel : SearchJobOrderModel
    {
        public SearchJobPostingModel() : base()
        {
        }

        public SearchJobPostingModel(DateTime? start, DateTime? end) : base(start, end)
        {
        }

        [WfmResourceDisplayName("Common.Status")]
        public int sf_JobPostingStatusId { get; set; }

        [WfmResourceDisplayName("Common.IsSubmitted")]
        public bool sf_IsSubmitted { get; set; }

        [WfmResourceDisplayName("Common.IsPublished")]
        public bool sf_IsPublished { get; set; }

        [WfmResourceDisplayName("Common.SubmittedBy")]
        public int sf_SubmittedBy { get; set; }
    }
}
