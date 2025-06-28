using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wfm.Core.Domain.Candidates
{
    public class AttendanceList
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusName { get; set; }
        public string EmployeeId { get; set; }
    }
}