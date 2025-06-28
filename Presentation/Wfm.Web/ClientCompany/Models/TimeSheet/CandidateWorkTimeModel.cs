using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.TimeSheet
{
    public class CandidateWorkTimeModel : BaseWfmEntityModel
    {
        public CandidateWorkTimeModel()
        {
            AvailableCandidateWorkTimeStatus = new List<SelectListItem>();
        }

        public Guid CandidateGuid { get; set; }
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
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

        [WfmResourceDisplayName("Common.Shift")]
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

        [Required]
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
        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.Source")]
        public string Source { get; set; }



        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.LateInTimeInMinutes")]
        public decimal LateInTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.EarlyOutTimeInMinutes")]
        public decimal EarlyOutTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.GracePeriodInMinutes")]
        public decimal GracePeriodInMinutes { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.IsStrictSchedule")]
        public bool IsStrictSchedule { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.MealTimeInMinutes")]
        public decimal MealTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.BreakTimeInMinutes")]
        public decimal BreakTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.RoundingIntervalInMinutes")]
        public decimal RoundingIntervalInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.TotalAbsenceTimeInMinutes")]
        public decimal TotalAbsenceTimeInMinutes { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.JobOrderDurationInMinutes")]
        public decimal JobOrderDurationInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.JobOrderDurationInHours")]
        public decimal JobOrderDurationInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockTimeInMinutes")]
        public decimal ClockTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockTimeInHours")]
        public decimal ClockTimeInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.LateOutTimeInMinutes")]
        public decimal LateOutTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.LateOutTimeInHours")]
        public decimal LateOutTimeInHours { get; set; }


        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.GrossWorkTimeInMinutes")]
        public decimal GrossWorkTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.GrossWorkTimeInHours")]
        public decimal GrossWorkTimeInHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.NetWorkTimeInMinutes")]
        public decimal NetWorkTimeInMinutes { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.TotalHours")]
        public decimal NetWorkTimeInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.AdjustmentInMinutes")]
        public decimal AdjustmentInMinutes { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        [WfmResourceDisplayName("Common.UpdatedBy")]
        public int UpdatedBy { get; set; }


        [WfmResourceDisplayName("Common.ApprovedBy")]
        public int ApprovedBy { get; set; }
        [WfmResourceDisplayName("Common.ApprovedBy")]
        public string ApprovedByName { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ApprovedOnUtc")]
        public DateTime? ApprovedOnUtc { get; set; }


        public bool AllowSuperVisorModifyWorkTime { get; set; }
        //public bool AllowDailyApproval { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.OvertimeMessage")]
        public string OvertimeMessage { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.OtherAlerts")]
        public string OtherAlerts { get; set; }

        public int Payroll_BatchId { get; set; }

        public bool IsCurrentWeek
        {
            get
            {
                return DateTime.Today.AddDays(7 - (int)(DateTime.Today.DayOfWeek)).Equals( // next Sunday 
                    JobStartDateTime.Date.AddDays(7 - (int)(JobStartDateTime.Date.DayOfWeek)));
            }
        }

        public IList<SelectListItem> AvailableCandidateWorkTimeStatus { get; set; }
        public IList<SelectListItem> AvailableJobOrders { get; set; }
        public IList<SelectListItem> AvaliableCandidates { get; set; }
        public IList<SelectListItem> AvaliableStartDateTimes { get; set; }
    }
}