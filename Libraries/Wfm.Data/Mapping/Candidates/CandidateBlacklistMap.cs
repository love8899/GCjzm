using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateBlacklistMap : EntityTypeConfiguration<CandidateBlacklist>
    {

        public CandidateBlacklistMap()
        {
            this.ToTable("CandidateBlacklist");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.Candidate)
                .WithMany()
                .HasForeignKey(c => c.CandidateId);
        }
    }
}
