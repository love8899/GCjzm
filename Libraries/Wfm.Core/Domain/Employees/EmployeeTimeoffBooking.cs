using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeeTimeoffBooking : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int? EmployeeTimeoffBalanceId { get; set; }
        public int? EmployeeTimeoffTypeId { get; set; }
        public decimal BookedTimeoffInHours { get; set; }
        public decimal ApprovedTimeoffInHours { get; set; }
        public bool? IsRejected { get; set; }
        public DateTime TimeOffStartDateTime { get; set; }
        public DateTime TimeOffEndDateTime { get; set; }
        public int BookedByAccountId { get; set; }
        public int? ApprovedByAccountId { get; set; }
        public string Note { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual EmployeeTimeoffType EmployeeTimeoffType { get; set; }
        public virtual EmployeeTimeoffBalance EmployeeTimeoffBalance { get; set; }
        public virtual Account BookedByAccount { get; set; }
        public virtual Account ApprovedByAccount { get; set; }
    }
}
