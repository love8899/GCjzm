using System;


namespace Wfm.Core.Domain.Candidates
{
    public class EmployeeTimeChartHistory
    {
        public int Id { get; set; }
        public Guid CandidateGuid { get; set; }
        public string EmployeeNumber { get; set; } 
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int FranchiseId { get; set; }
        public string FranchiseName { get; set; }
        public int CompanyId { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
        public int CompanyLocationId { get; set; }
        public string LocationName { get; set; }
        public int CompanyDepartmentId { get; set; }
      
        public string DepartmentName { get; set; }
        public int CompanyContactId { get; set; }
        public string ContactName { get; set; }
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }
        public string JobTitle { get; set; }
        public string PositionCode { get; set; }
        public string ShiftCode { get; set; }
        public string RateCode { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public DateTime SundayDate { get; set; }
        public string SundayName { get { return this.SundayDate.ToString("MM/dd/yyyy"); } }
        public decimal Sunday { get; set; }
        public bool SundayStatus { get; set; }
        public DateTime MondayDate { get; set; }
        public string MondayName { get { return this.MondayDate.ToString("MM/dd/yyyy"); } }
        public decimal Monday { get; set; }
        public bool MondayStatus { get; set; }
        public DateTime TuesdayDate { get; set; }
        public string TuesdayName { get { return this.TuesdayDate.ToString("MM/dd/yyyy"); } }
        public decimal Tuesday { get; set; }
        public bool TuesdayStatus { get; set; }
        public DateTime WednesdayDate { get; set; }
        public string WednesdayName { get { return this.WednesdayDate.ToString("MM/dd/yyyy"); } }
        public decimal Wednesday { get; set; }
        public bool WednesdayStatus { get; set; }
        public DateTime ThursdayDate { get; set; }
        public string ThursdayName { get { return this.ThursdayDate.ToString("MM/dd/yyyy"); } }
        public decimal Thursday { get; set; }
        public bool ThursdayStatus { get; set; }
        public DateTime FridayDate { get; set; }
        public string FridayName { get { return this.FridayDate.ToString("MM/dd/yyyy"); } }
        public decimal Friday { get; set; }
        public bool FridayStatus { get; set; }
        public DateTime SaturdayDate { get; set; }
        public string SaturdayName { get { return this.SaturdayDate.ToString("MM/dd/yyyy"); } }
        public decimal Saturday { get; set; }
        public bool SaturdayStatus { get; set; }
        public Nullable<decimal> SubTotalHours { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public Nullable<decimal> RegularHours { get; set; }
        public decimal? RegularBillingRate { get; set; }
        public decimal? OvertimeBillingRate { get; set; }
        public decimal? BillingTaxRate { get; set; }
        public string Note { get; set; }
        public bool WeeklyStatus { get; set; }
        public string FullWeekName { get { return String.Format("{0} / {1} ( {2} - {3} )", this.WeekOfYear, this.Year, this.SundayDate.ToString("MM/dd/yyyy"), this.SaturdayDate.ToString("MM/dd/yyyy")); } }
        public string ApprovedBy { get; set; }
    }
}