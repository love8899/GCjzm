using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class TransportationMap : EntityTypeConfiguration<Transportation>
    {
        public TransportationMap()
        {
            this.ToTable("Transportation");
            this.HasKey(c => c.Id);

            this.Property(c => c.TransportationName).IsRequired().HasMaxLength(255);
        }
    }
}
