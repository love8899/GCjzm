using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class JobOrderTestCategoryMap :EntityTypeConfiguration<JobOrderTestCategory>
    {
        public JobOrderTestCategoryMap()
        {
            this.ToTable("JobOrderTestCategory");
            this.HasKey(c => c.Id);
        }
    }
}
