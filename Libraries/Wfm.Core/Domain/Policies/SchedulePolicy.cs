using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Policies
{
    public class SchedulePolicy : BaseEntity
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }

        public int MealPolicyId { get; set; }
        public int BreakPolicyId { get; set; }
        public int RoundingPolicyId { get; set; }

        public bool IsStrictSchedule { get; set; }
        public int OvertimeGracePeriodInMinutes { get; set; }

        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Company Company { get; set; }
        public virtual MealPolicy MealPolicy { get; set; }
        public virtual BreakPolicy BreakPolicy { get; set; }
        public virtual RoundingPolicy RoundingPolicy { get; set; }
    }
}
