using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Logging;

namespace Wfm.Data.Mapping.Logging
{
    public partial class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.ToTable("Log");
            this.HasKey(l => l.Id);
            this.Property(l => l.ShortMessage).IsRequired();
            this.Property(l => l.IpAddress).HasMaxLength(200);

            this.Ignore(l => l.LogLevel);

            this.HasOptional(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.AccountId)
            .WillCascadeOnDelete(true);

        }
    }
}