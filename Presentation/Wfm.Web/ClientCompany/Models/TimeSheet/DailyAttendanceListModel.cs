using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.TimeSheet
{
    public class DailyAttendanceListModel
    {
        public int CandidateJobOrderId { get; set; }
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        public string JobTitleAndId { get; set; }

        [WfmResourceDisplayName("Common.ShiftStartTime")]
        public DateTime ShiftStartTime { get; set; }
        [WfmResourceDisplayName("Common.ShiftEndTime")]
        public DateTime ShiftEndTime { get; set; }

        public string Status { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string Location { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string Department { get; set; }
        public decimal TotalHoursWorked { get; set; }
      
    }
}