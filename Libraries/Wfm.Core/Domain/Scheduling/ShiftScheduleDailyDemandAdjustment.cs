using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Scheduling
{
    public class ShiftScheduleDailyDemandAdjustment: BaseEntity
    {
        public int ShiftScheduleDailyId { get; set; }
        public int CompanyJobRoleId { get; set; }
        public int AdjustedMandantoryRequiredCount { get; set; }
        public int AdjustedContingencyRequiredCount { get; set; }
        public virtual ShiftScheduleDaily ShiftScheduleDaily { get; set; }
        public virtual CompanyJobRole CompanyJobRole { get; set; }
    }
}
