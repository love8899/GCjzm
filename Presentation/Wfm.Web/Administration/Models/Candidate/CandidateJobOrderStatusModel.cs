using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateJobOrderStatusValidator))]
    public partial class CandidateJobOrderStatusModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.CandidateJobOrderStatus.Fields.StatusName")]
        public string StatusName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CandidateJobOrderStatus.Fields.CanBeScheduled")]
        public bool? CanBeScheduled { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CandidateJobOrderStatus.Fields.TriggersEmail")]
        public bool? TriggersEmail { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}