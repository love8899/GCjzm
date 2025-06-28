using System.Collections.Generic;
using Wfm.Admin.Models.JobOrder;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate Pipeline model
    /// </summary>
    public class CandidatePipelineModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        public IList<JobOrderModel> JobOrderModels { get; set; }

    }
}