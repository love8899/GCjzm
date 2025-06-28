using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.TimeSheet
{
    public class TimeSheetDetails
    {
        public int CompanyId { get; set; }
        public int VendorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SupervisorId { get; set; }
        public string Supervisor { get; set; }
        public decimal SubmittedHours { get; set; }
        public decimal ApprovedHours { get; set; }
        public decimal TotalHours { get; set; }
    }
}
