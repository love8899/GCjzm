using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class JobOrderTypeMap : EntityTypeConfiguration<JobOrderType>
    {
        public JobOrderTypeMap()
        {
            this.ToTable("JobOrderType");
            this.HasKey(c => c.Id);

            this.Property(c => c.JobOrderTypeName).IsRequired().HasMaxLength(255);
        }
    }
}
