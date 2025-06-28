using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Media;

namespace Wfm.Data.Mapping.Media
{
  
    class DocumentTypeMap : EntityTypeConfiguration<DocumentType>
    {
        public DocumentTypeMap()
        {
            this.ToTable("DocumentType");
            this.HasKey(c => c.Id);
             
            this.Property(c => c.TypeName).IsRequired().HasMaxLength(255);

            this.Property(c => c.InternalCode).IsRequired().HasMaxLength(50);
        }
    }
}
