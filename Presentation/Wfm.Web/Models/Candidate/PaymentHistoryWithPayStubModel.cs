using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wfm.Web.Models.Candidate
{
    public class PaymentHistoryWithPayStubModel
    {
        public Guid CandidatePaymentHistoryGuid { get; set; }
        public int CandidateId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PayPeriodStartDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
    }
}