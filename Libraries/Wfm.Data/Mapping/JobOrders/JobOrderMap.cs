using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class JobOrderMap :EntityTypeConfiguration<JobOrder>
    {
        public JobOrderMap()
        {
            this.ToTable("JobOrder");
     
            this.HasKey(jo => jo.Id);

            this.Property(jo => jo.JobOrderGuid).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(jo => jo.JobTitle).HasMaxLength(255);
            
            this.Property(jo => jo.BillingRateCode).HasMaxLength(105);

            this.HasRequired(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);
        }
    }
}
