using System;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.Companies
{
    public class CompanyLocationOvertimeRule : BaseEntity
    {
        public int CompanyLocationId { get; set; }
        public int OvertimeRuleSettingId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }

        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual OvertimeRuleSetting OvertimeRuleSetting { get; set; }
    }
}
