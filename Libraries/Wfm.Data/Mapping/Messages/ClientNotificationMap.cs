using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Messages
{
    public class ClientNotificationMap : EntityTypeConfiguration<ClientNotification>
    {
        public ClientNotificationMap() 
        {
            this.ToTable("ClientNotification");

            this.HasKey(x => x.Id);
            
            this.HasRequired(x => x.MessageTemplate)
                .WithMany()
                .HasForeignKey(x => x.MessageTemplateId);

            this.HasRequired(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);
            
            this.HasRequired(x => x.AccountRole)
                .WithMany()
                .HasForeignKey(x => x.AccountRoleId);
        }
    }
}
