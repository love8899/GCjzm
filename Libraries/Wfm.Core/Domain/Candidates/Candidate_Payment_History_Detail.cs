using System;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Core.Domain.Candidates
{
    public partial class Candidate_Payment_History_Detail
    {
        public int Id { get; set; }
        public int Payment_HistoryId { get; set; }
        public int Payroll_ItemId { get; set; }
        public Nullable<decimal> Unit { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> YTD_Unit { get; set; }
        public Nullable<decimal> YTD_Amount { get; set; }
        public Nullable<int> JobOrder_Id { get; set; }

        public virtual Candidate_Payment_History Candidate_Payment_History { get; set; }
        public virtual Payroll_Item Payroll_Item { get; set; }
    }

    public class PaymentDetail
    {
     //   public int Payment_HistoryId { get; set; }
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
        public DateTime? ItemDate { get; set; }
        public string WSIB_Code { get; set; }
        public decimal? WSIB_Rate { get; set; }
    }
}
