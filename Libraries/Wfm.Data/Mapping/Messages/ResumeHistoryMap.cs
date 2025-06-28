using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;


namespace Wfm.Data.Mapping.Candidates
{
    public partial class ResumeHistoryMap : EntityTypeConfiguration<ResumeHistory>
    {
        public ResumeHistoryMap()
        {
            this.ToTable("ResumeHistory");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Resume)
                .WithMany(r => r.ResumeHistories)
                .HasForeignKey(x => x.ResumeId);
        }
    }
}
