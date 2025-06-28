using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.TimeSheet
{
    public class TimeSheetSummary
    {        
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal SubmittedHours { get; set; }
        public decimal ApprovedHours { get; set; }
        public decimal TotalHours { get; set; }
        public decimal InvoicedHours { get; set; }
        public decimal NotInvoicedHours { get; set; }
    }
}
