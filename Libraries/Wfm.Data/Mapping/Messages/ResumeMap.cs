using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Messages
{
    public partial class ResumeMap : EntityTypeConfiguration<Resume>
    {
        public ResumeMap()
        {
            this.ToTable("Resume");
            this.HasKey(x => x.Id);

            this.Property(x => x.From).IsRequired().HasMaxLength(500);
            this.Property(x => x.FromName).HasMaxLength(500);
            this.Property(x => x.To).IsRequired();
            this.Property(x => x.ToName).HasMaxLength(500);
            this.Property(x => x.Subject).HasMaxLength(1600);
        }
    }
}
