using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.TimeSheet
{
    public class OneWeekFollowUpReportData
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string LocationName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RecruiterId { get; set; }
        public int OwnerId { get; set; }

    }
}
