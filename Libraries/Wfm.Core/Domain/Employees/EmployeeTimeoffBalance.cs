using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeeTimeoffBalance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int EmployeeTimeoffTypeId { get; set; }
        public decimal EntitledTimeoffInHours { get; set; }
        public decimal? LatestBalanceInHours { get; set; }
        public virtual EmployeeTimeoffType EmployeeTimeoffType { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
