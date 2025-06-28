using Wfm.Core.Domain.Tests;

namespace Wfm.Core.Domain.JobOrders
{
    public class JobOrderTestCategory : BaseEntity
    {
        public int JobOrderId { get; set; }
        public int TestCategoryId { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }

        public virtual JobOrder JobOrder { get; set; }
        public virtual TestCategory TestCategory { get; set; }
    }
}