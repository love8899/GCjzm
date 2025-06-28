using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Shared.Models.JobPosting
{
    public class PlacementRejectionModel
    {
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public Guid? AssociatedGuid { get; set; }
        public int? AssociatedId { get; set; }
        public bool IsBanned { get; set; }
        public string BannedReason { get; set; }
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
