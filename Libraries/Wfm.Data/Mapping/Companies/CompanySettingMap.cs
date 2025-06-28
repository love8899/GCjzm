using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Configuration
{
    public partial class CompanySettingMap : EntityTypeConfiguration<CompanySetting>
    {
        public CompanySettingMap()
        {
            this.ToTable("CompanySetting");
            this.HasKey(s => s.Id);
            this.Property(s => s.CompanyId).IsRequired();
            this.Property(s => s.Name).IsRequired().HasMaxLength(255);
            this.Property(s => s.Value).IsRequired().HasMaxLength(4000);
        }
    }
}