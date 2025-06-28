using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class JobOrderCategoryMap : EntityTypeConfiguration<JobOrderCategory>
    {
        public JobOrderCategoryMap()
        {
            this.ToTable("JobOrderCategory");
            this.HasKey(joc => joc.Id);

            this.Property(joc => joc.CategoryName).IsRequired().HasMaxLength(255);
        }
    }
}
