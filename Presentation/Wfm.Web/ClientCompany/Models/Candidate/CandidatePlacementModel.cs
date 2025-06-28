using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.Candidate
{
    public class CandidatePlacementModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public int ShiftId { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public string ShiftName { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string DepartmentName { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public int CompanyContactId { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public string ContactName { get; set; }
        [WfmResourceDisplayName("Common.ShiftStartTime")]
        public DateTime StartTime { get; set; }
        [WfmResourceDisplayName("Common.ShiftEndTime")]
        public DateTime EndTime { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateJobOrderStatusId { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
    }
}