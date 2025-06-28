using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Features;


namespace Wfm.Data.Mapping.Features
{
    public partial class FeatureMap : EntityTypeConfiguration<Feature>
    {
        public FeatureMap()
        {
            this.ToTable("Feature");
            this.HasKey(c => c.Id);

            this.Property(c => c.Area).IsRequired().HasMaxLength(20);
            this.Property(c => c.Code).IsRequired().HasMaxLength(20);
            this.Property(c => c.Name).IsRequired().HasMaxLength(255);
        }
    }
}
