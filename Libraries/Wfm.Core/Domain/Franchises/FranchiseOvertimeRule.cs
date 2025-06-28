using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.Franchises
{
    public class FranchiseOvertimeRule : BaseEntity
    {
        public int FranchiseId { get; set; }
        public int OvertimeRuleSettingId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }

        public virtual Franchise Franchise { get; set; }
        public virtual OvertimeRuleSetting OvertimeRuleSetting { get; set; }
    }
}
