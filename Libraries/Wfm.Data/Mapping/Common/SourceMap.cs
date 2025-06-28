using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class SourceMap : EntityTypeConfiguration<Source>
    {
        public SourceMap()
        {
            this.ToTable("Source");
            this.HasKey(c => c.Id);

            this.Property(c => c.SourceName).IsRequired().HasMaxLength(255);
        }
    }
}
