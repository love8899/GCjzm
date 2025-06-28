using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Models.JobOrder;

namespace Wfm.Web.Models.Candidate
{
    public partial class CandidateAppliedJobModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Web.Common.Address")]
        public string Address { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public string City { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        public string PostalCode { get; set; }
    }

}