using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Logging;

namespace Wfm.Data.Mapping.Logging
{
    public partial class ActivityLogTypeMap : EntityTypeConfiguration<ActivityLogType>
    {
        public ActivityLogTypeMap()
        {
            this.ToTable("ActivityLogType");
            this.HasKey(c => c.Id);

            this.Property(c => c.ActivityLogTypeName).IsRequired().HasMaxLength(255);
        }
    }
}
