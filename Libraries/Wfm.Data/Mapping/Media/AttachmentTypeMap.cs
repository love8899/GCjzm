using System.Data.Entity.ModelConfiguration;

namespace Wfm.Core.Domain.Media
{
    class AttachmentTypeMap : EntityTypeConfiguration<AttachmentType>
    {
        public AttachmentTypeMap()
        {
            this.ToTable("AttachmentType");
            this.HasKey(c => c.Id);

            this.Property(c => c.TypeName).IsRequired().HasMaxLength(255);
        }
    }
}
