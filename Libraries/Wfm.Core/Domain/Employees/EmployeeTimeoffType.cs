using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeeTimeoffType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Paid { get; set; }
        public bool IsActive { get; set; }
        public int? DefaultAnnualEntitlementInHours { get; set; }
        public bool AllowNegative { get; set; }
        public int EmployeeTypeId { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public ICollection<EmployeeTimeoffBalance> EmployeeTimeoffBalances { get; set; }
    }
}
