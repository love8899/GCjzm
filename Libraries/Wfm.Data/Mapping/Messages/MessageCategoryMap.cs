using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Tests
{
    public partial class MessageCategoryMap : EntityTypeConfiguration<MessageCategory>
    {
        public MessageCategoryMap()
        {
            this.ToTable("MessageCategory");
            this.HasKey(c => c.Id);

            this.Property(c => c.CategoryName).IsRequired().HasMaxLength(255);
        }
    }
}
