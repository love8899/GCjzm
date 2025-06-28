using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Logging;

namespace Wfm.Data.Mapping.Logging
{
    public partial class ActivityLogMap : EntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            this.ToTable("ActivityLog");
            this.HasKey(c => c.Id);

        }
    }
}
