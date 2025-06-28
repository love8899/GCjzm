using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;


namespace Wfm.Client.Models.Rescheduling
{
    public class ReschedulingModel
    {
        public int OrigId { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.EmployeeName")]
        public string EmployeeName { get; set; }

        public string PictureThumbnailUrl { get; set; }

        [WfmResourceDisplayName("Common.JobOrder")]
        public int JobOrderId { get; set; }

        public Guid JobOrderGuid { get; set; }

        [Required]
        public int FranchiseId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.Location")]
        public int LocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int DepartmentId { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.Position")]
        public int PositionId { get; set; }

        [Required]
        public int ShiftId { get; set; }

        [Required]
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Supervisor")]
        public int CompanyContactId { get; set; }
        public string Supervisor { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [WfmResourceDisplayName("Common.StartTime")]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [WfmResourceDisplayName("Common.EndTime")]
        public DateTime EndTime { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime EndDate { get; set; }

        public string PayRate { get; set; }

        public DateTime? PunchIn { get; set; }
        public int WorkTimeId { get; set; }
    }
}