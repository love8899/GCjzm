using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.TimeSheet
{
   public class DailyAttendanceList
   {
       public int CandidateJobOrderId { get; set; }
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }

        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string JobTitleAndId { get; set; }
        
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }

        public string Status { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string VendorName { get; set; }
        public decimal TotalHoursWorked { get; set; }
    }
}
