using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;

namespace Wfm.Data.Mapping.Messages
{
    public partial class MessageTemplateMap : EntityTypeConfiguration<MessageTemplate>
    {
        public MessageTemplateMap()
        {
            this.ToTable("MessageTemplate");
            this.HasKey(mt => mt.Id);

            this.Property(mt => mt.TagName).IsRequired().HasMaxLength(200);
            this.Property(mt => mt.CCEmailAddresses).HasMaxLength(500);
            this.Property(mt => mt.BccEmailAddresses).HasMaxLength(200);
            this.Property(mt => mt.Subject).HasMaxLength(1000);

            this.Property(mt => mt.EmailAccountId).IsRequired();
        }
    }
}
