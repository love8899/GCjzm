using FluentValidation.Attributes;
using System;
using Wfm.Admin.Validators.Employee;


namespace Wfm.Admin.Models.Employee
{
    public class PaymentDetailModel
    {
        public int Payment_HistoryId { get; set; }
        public int Payroll_ItemId { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public string DisplayCode { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SubTypeCode { get; set; }
        public decimal? Unit { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? YTD_Unit { get; set; }
        public decimal? YTD_Amount { get; set; }
        public int? JobOrder_Id { get; set; }
        public DateTime? ItemDate {get; set;}
        public string WSIB_Code {get; set;}
        public decimal? WSIB_Rate { get; set; }
    }
}
