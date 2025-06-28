using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Candidates
{
    public class PaymentHistoryWithPayStub
    {
        public Guid CandidatePaymentHistoryGuid { get; set; }
        public int CandidateId { get; set; }
        public DateTime PaymentDate { get; set; }
        public byte[] Paystub { get; set; }
        public DateTime PayPeriodStartDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
    }

    public class PaystubPassword
    {
        public int? Month { get; set; }
        public int? Day { get; set; }
        public string Last_3_SIN { get; set; }
        public string PayStubPassword { get; set; }
    }
}
