using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Configuration;

namespace Wfm.Data.Mapping.Configuration
{
    public partial class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name).IsRequired().HasMaxLength(255);
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}