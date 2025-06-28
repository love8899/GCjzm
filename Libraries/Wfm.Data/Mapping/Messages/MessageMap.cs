using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Messages
{
    public partial class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            this.ToTable("Message");
            this.HasKey(m => m.Id);

            this.Property(m => m.MessageHistoryId).IsRequired();
            this.Property(m => m.Recipient).IsRequired().HasMaxLength(500);
            this.Property(m => m.ByEmail).IsRequired();
            this.Property(m => m.ByMessage).IsRequired();
            this.Property(m => m.IsRead).IsRequired();
        }
    }
}
