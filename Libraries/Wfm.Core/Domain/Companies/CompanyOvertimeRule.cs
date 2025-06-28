using System;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.Companies
{
    public class CompanyOvertimeRule : BaseEntity
    {
        public int CompanyId { get; set; }
        public int OvertimeRuleSettingId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }

        public virtual Company Company { get; set; }
        public virtual OvertimeRuleSetting OvertimeRuleSetting { get; set; }
    }
}
