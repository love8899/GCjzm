using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Security;

namespace Wfm.Data.Mapping.Security
{
    public partial class PermissionRecordMap : EntityTypeConfiguration<PermissionRecord>
    {
        public PermissionRecordMap()
        {
            this.ToTable("PermissionRecord");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(255);
            this.Property(c => c.SystemName).IsRequired();
            this.Property(c => c.Category).IsRequired().HasMaxLength(255);

            this.HasMany(c => c.AccountRoles)
                .WithMany(cr => cr.PermissionRecords)
                .Map(m => m.ToTable("PermissionRecord_Role_Mapping"));
        }
    }
}
