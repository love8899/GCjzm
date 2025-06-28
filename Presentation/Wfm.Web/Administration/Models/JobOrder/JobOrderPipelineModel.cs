using System.Collections.Generic;
using Wfm.Web.Framework;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderPipelineModel
    {
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        public IList<CandidateModel> CandidateModels { get; set; }
    }
}