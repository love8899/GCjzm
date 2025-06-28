using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Candidate
{
    public partial class CandidateJobOrderStatusModel : BaseWfmEntityModel
    {
        public string StatusName { get; set; }

        public string Description { get; set; }

        public bool? CanBeScheduled { get; set; }

        public bool? TriggersEmail { get; set; }

        public bool IsActive { get; set; }

        public string Note { get; set; }

    }

}