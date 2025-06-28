using System;

namespace Wfm.Admin.Models.Candidate
{
    public partial class PipelineProfile
    {
        public int JobOrderId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int[] StatusIds { get; set; }
    }

}
