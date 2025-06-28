using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public class CandidatePasswordHistoryMap: EntityTypeConfiguration<CandidatePasswordHistory>
    {
        public CandidatePasswordHistoryMap()
        {
            this.ToTable("CandidatePasswordHistory");
            this.HasKey(x => x.Id);
            this.Property(x => x.CandidateId).IsRequired();

        }
    }
}
