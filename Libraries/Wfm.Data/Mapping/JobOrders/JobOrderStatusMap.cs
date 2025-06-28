using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class JobOrderStatusMap : EntityTypeConfiguration<JobOrderStatus>
    {
        public JobOrderStatusMap()
        {
            this.ToTable("JobOrderStatus");
            this.HasKey(jos => jos.Id);

            this.Property(jos => jos.JobOrderStatusName).IsRequired().HasMaxLength(255);
        }
    }
}
