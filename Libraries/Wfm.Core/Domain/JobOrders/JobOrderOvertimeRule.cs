using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.JobOrders
{
    public class JobOrderOvertimeRule : BaseEntity
    {
        public int JobOrderId { get; set; }
        public int OvertimeRuleSettingId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }

        public virtual JobOrder JobOrder { get; set; }
        public virtual OvertimeRuleSetting OvertimeRuleSetting { get; set; }
    }
}
