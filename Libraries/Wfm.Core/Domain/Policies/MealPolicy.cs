using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Policies
{
    public class MealPolicy : BaseEntity
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public int MealTimeInMinutes { get; set; }
        public decimal MinWorkHours { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Company Company { get; set; }
    }
}
