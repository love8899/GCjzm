using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class DailyAttendanceModel : BaseWfmEntityModel
    {
        public int CandidateJobOrderId { get; set; } 
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Depatment { get; set; }
        public string Supervisor { get; set; }
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        //public DateTime? ClockIn { get; set; }
        //public DateTime? ClockOut { get; set; }
        //public decimal? JobDuration { get; set; }
        //public decimal? GrossHours { get; set; }
        //public decimal? Adjustment { get; set; }
        //public decimal? NetHours { get; set; }
    }
}
