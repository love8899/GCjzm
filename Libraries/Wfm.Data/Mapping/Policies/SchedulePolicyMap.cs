using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Policies;

namespace Wfm.Data.Mapping.Policies
{
    public partial class SchedulePolicyMap : EntityTypeConfiguration<SchedulePolicy>
    {
        public SchedulePolicyMap()
        {
            this.ToTable("SchedulePolicy");
            this.HasKey(s => s.Id);

            this.Property(s => s.Name).HasMaxLength(255);


            this.HasRequired(c => c.Company)
                .WithMany(c => c.SchedulePolicies)
                .HasForeignKey(c => c.CompanyId);
        }
    }
}
