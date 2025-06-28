using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.TimeSheet
{
    public class DailyTimeSheetModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
      
        [WfmResourceDisplayName("Common.Status")]
        public int CandidateWorkTimeStatusId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }
        [WfmResourceDisplayName("Common.Name")]
        public string EmployeeName { get { return String.Concat(EmployeeFirstName, " ", EmployeeLastName); } }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int ShiftId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public int CompanyContactId { get; set; }

        [WfmResourceDisplayName("Common.JobStartDateTime")]
        public DateTime JobStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.JobEndDateTime")]
        public DateTime JobEndDateTime { get; set; }

        [WfmResourceDisplayName("Common.Year")]
        public int Year { get; set; }
        [WfmResourceDisplayName("Common.WeekOfYear")]
        public int WeekOfYear { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockIn")]
        public DateTime? ClockIn { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockOut")]
        public DateTime? ClockOut { get; set; }   
       
        [WfmResourceDisplayName("Common.TotalHours")]
        public decimal NetWorkTimeInHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockTimeInHours")]
        public decimal ClockTimeInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.AdjustmentInMinutes")]
        public decimal AdjustmentInMinutes { get; set; }
       
        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }
    }
}
