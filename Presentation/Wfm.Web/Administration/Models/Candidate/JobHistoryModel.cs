using System;
using System.Collections.Generic;
using System.Globalization;
using Wfm.Services.Tasks;


namespace Wfm.Admin.Models.Candidate
{
    public class JobHistoryModel
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public DateTime SundayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear); } }
        public decimal Sunday { get; set; }
        public DateTime MondayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(1); } }
        public decimal Monday { get; set; }
        public DateTime TuesdayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(2); } }
        public decimal Tuesday { get; set; }
        public DateTime WednesdayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(3); } }
        public decimal Wednesday { get; set; }
        public DateTime ThursdayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(4); } }
        public decimal Thursday { get; set; }
        public DateTime FridayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(5); } }
        public decimal Friday { get; set; }
        public DateTime SaturdayDate { get { return DateService.FirstDateOfWeek(Year, WeekOfYear).AddDays(6); } }
        public decimal Saturday { get; set; }
        public Nullable<decimal> SubTotalHours { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public Nullable<decimal> RegularHours { get { return SubTotalHours - OTHours; } }
    }

}
