using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class GenderMap : EntityTypeConfiguration<Gender>
    {
        public GenderMap()
        {
            this.ToTable("Gender");
            this.HasKey(c => c.Id);

            this.Property(c => c.GenderName).IsRequired().HasMaxLength(255);
        }
    }
}
