using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Blogs;

namespace Wfm.Data.Mapping.Blogs
{
    public partial class BlogPostMap : EntityTypeConfiguration<BlogPost>
    {
        public BlogPostMap()
        {
            this.ToTable("BlogPost");
            this.HasKey(bp => bp.Id);
            this.Property(bp => bp.Title).IsRequired();
            this.Property(bp => bp.Body).IsRequired();
            this.Property(bp => bp.MetaKeywords).HasMaxLength(400);
            this.Property(bp => bp.MetaTitle).HasMaxLength(400);

            this.HasRequired(bp => bp.Language)
                .WithMany()
                .HasForeignKey(bp => bp.LanguageId).WillCascadeOnDelete(true);
        }
    }
}