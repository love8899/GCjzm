using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    public class CandidateWorkTimeBaseModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateWorkTimeStatusId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Common.JobShift")]
        public int ShiftId { get; set; }

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

        [WfmResourceDisplayName("Common.JobStartDateTime")]
        public DateTime JobStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.JobEndDateTime")]
        public DateTime JobEndDateTime { get; set; }

        [WfmResourceDisplayName("Common.Year")]
        public int Year { get; set; }
        [WfmResourceDisplayName("Common.WeekOfYear")]
        public int WeekOfYear { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.NetWorkTimeInHours")]
        public decimal NetWorkTimeInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.InvoiceDate")]
        public DateTime? InvoiceDate { get; set; }

        public int FranchiseId { get; set; }
    }
}