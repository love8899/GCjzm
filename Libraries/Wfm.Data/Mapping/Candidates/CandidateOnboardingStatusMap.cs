using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateOnboardingStatusMap : EntityTypeConfiguration<CandidateOnboardingStatus>
    {
        public CandidateOnboardingStatusMap()
        {
            this.ToTable("CandidateOnboardingStatus");
            this.HasKey(c => c.Id);

            this.Property(c => c.StatusName).HasMaxLength(255);
        }
    }
}
