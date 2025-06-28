using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Blogs;

namespace Wfm.Data.Mapping.Blogs
{
    public partial class BlogCommentMap : EntityTypeConfiguration<BlogComment>
    {
        public BlogCommentMap()
        {
            this.ToTable("BlogComment");
            this.HasKey(pr => pr.Id);

            this.HasRequired(bc => bc.BlogPost)
                .WithMany(bp => bp.BlogComments)
                .HasForeignKey(bc => bc.BlogPostId);

            this.HasRequired(bc => bc.Account)
                .WithMany()
                .HasForeignKey(bc => bc.AccountId);
        }
    }
}