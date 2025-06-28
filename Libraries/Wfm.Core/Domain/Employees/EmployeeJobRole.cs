using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeeJobRole : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int CompanyJobRoleId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsPrimary { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual CompanyJobRole CompanyJobRole { get; set; }
    }
}
