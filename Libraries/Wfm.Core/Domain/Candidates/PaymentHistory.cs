using System;
using System.Collections.Generic;


namespace Wfm.Core.Domain.Candidates
{
    public class PaymentHistory
    {
        public Guid CandidatePaymentHistoryGuid { get; set; }
        public int PaymentHistory_Id { get; set; }

        public int CandidateId { get; set; }
        public string Year { get; set; }
        public int PayrollBatch_Id { get; set; }
        public string Company { get; set; }
        public string PayGroup { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ChequeNumber { get; set; }
        public string DirectDepositNumber { get; set; }
        public string ProvinceCode { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }

        public bool IsEmailed { get; set; }
        public bool IsPrinted { get; set; }

        public string Note { get; set; }
    }
}
