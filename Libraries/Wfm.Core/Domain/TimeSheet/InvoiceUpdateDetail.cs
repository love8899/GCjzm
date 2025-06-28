using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.TimeSheet
{
    public class InvoiceUpdateDetail
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int FranchiseId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string DepartmentName { get; set; }
        public string PositionCode { get; set; }
        public string ShiftCode { get; set; }
        public string RateCode { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public DateTime SundayDate { get; set; }
        public decimal Sunday { get; set; }
        public DateTime MondayDate { get; set; }
        public decimal Monday { get; set; }
        public DateTime TuesdayDate { get; set; }
        public decimal Tuesday { get; set; }
        public DateTime WednesdayDate { get; set; }
        public decimal Wednesday { get; set; }
        public DateTime ThursdayDate { get; set; }
        public decimal Thursday { get; set; }
        public DateTime FridayDate { get; set; }
        public decimal Friday { get; set; }
        public DateTime SaturdayDate { get; set; }
        public decimal Saturday { get; set; }
        public Nullable<decimal> SubTotalHours { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public Nullable<decimal> RegularHours { get; set; }
        public decimal? RegularBillingRate { get; set; }
        public decimal? OvertimeBillingRate { get; set; }
        public decimal? BillingTaxRate { get; set; }
    }
}
