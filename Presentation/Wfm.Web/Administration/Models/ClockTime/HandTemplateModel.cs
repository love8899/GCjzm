using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.ClockTime
{
    public class HandTemplateModel : BaseWfmEntityModel
    {
        public Guid HandTemplateGuid { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        // For view only
        [WfmResourceDisplayName("Common.SmartCard")]
        public string SmartCardUid { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        public int FranchiseId { get; set; }

        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
    }
}
