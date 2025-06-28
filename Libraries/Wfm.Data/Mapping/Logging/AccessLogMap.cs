using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Logging;

namespace Wfm.Data.Mapping.Logging
{
    public partial class AccessLogMap : EntityTypeConfiguration<AccessLog>
    {
        public AccessLogMap()
        {
            this.ToTable("AccessLog");
            this.HasKey(al => al.Id);

        }
    }
}
