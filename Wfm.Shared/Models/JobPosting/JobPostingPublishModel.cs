using System;
using System.ComponentModel.DataAnnotations;


namespace Wfm.Shared.Models.JobPosting
{
    public class JobPostingPublishModel
    {
        public int JobPostingId { get; set; }
        public int CompanyId { get; set; }

        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public Guid VendorGuid { get; set; } 

        //[Range(0, int.MaxValue)]
        [UIHint("RangedInteger")]
        public int Opening { get; set; }

        public string BillingRateCode { get; set; }

        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }

        public int Placed { get; set; }

        public int Shortage { get; set; }
    }
}
