using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Messages
{
    public class MessageTemplateAccountRoleMap : EntityTypeConfiguration<MessageTemplateAccountRole>
    {
        public MessageTemplateAccountRoleMap() 
        {
            this.ToTable("MessageTemplateAccountRole");
            this.HasKey(t => new { t.AccountRoleId, t.MessageTemplateId });
            this.HasRequired(m => m.MessageTemplate)
                .WithMany(c=>c.AccountRoles)
                .HasForeignKey(m => m.MessageTemplateId);
            this.HasRequired(m => m.AccountRole)
                .WithMany()
                .HasForeignKey(m => m.AccountRoleId);
        }
    }
}
