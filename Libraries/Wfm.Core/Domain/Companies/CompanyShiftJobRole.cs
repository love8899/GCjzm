using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyShiftJobRole : BaseEntity
    {
        public int CompanyShiftId { get; set; }
        public int CompanyJobRoleId { get; set; }
        public int MandantoryRequiredCount { get; set; }
        public int ContingencyRequiredCount { get; set; }
        public int? SupervisorId { get; set; }

        public virtual CompanyShift CompanyShift { get; set; }
        public virtual CompanyJobRole CompanyJobRole { get; set; }
        public virtual Account Supervisor { get; set; }
    }
}
