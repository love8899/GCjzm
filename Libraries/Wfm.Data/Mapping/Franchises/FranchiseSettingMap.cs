using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Data.Mapping.Configuration
{
    public partial class FranchiseSettingMap : EntityTypeConfiguration<FranchiseSetting>
    {
        public FranchiseSettingMap()
        {
            this.ToTable("FranchiseSetting");
            this.HasKey(s => s.Id);
            this.Property(s => s.FranchiseId).IsRequired();
            this.Property(s => s.Name).IsRequired().HasMaxLength(255);
            this.Property(s => s.Value).IsRequired().HasMaxLength(4000);
        }
    }
}