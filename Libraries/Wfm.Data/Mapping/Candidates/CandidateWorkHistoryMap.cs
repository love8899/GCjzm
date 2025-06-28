using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateWorkHistoryMap :EntityTypeConfiguration<CandidateWorkHistory>
    {
        public CandidateWorkHistoryMap()
        {
            this.ToTable("CandidateWorkHistory");
            this.HasKey(c => c.Id);

            this.Property(c => c.JobTitle).HasMaxLength(255);
        }
    }
}
