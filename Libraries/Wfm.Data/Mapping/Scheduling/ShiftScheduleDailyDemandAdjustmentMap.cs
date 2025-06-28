using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class ShiftScheduleDailyDemandAdjustmentMap : EntityTypeConfiguration<ShiftScheduleDailyDemandAdjustment>
    {
        public ShiftScheduleDailyDemandAdjustmentMap()
        {
            this.ToTable("ShiftScheduleDailyDemandAdjustment");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.ShiftScheduleDaily)
                .WithMany(c => c.Adjustments)
                .HasForeignKey(c => c.ShiftScheduleDailyId);
            this.HasRequired(c => c.CompanyJobRole)
                .WithMany()
                .HasForeignKey(c => c.CompanyJobRoleId);
        }
    }
}
