using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateJobOrderMap :EntityTypeConfiguration<CandidateJobOrder>
    {
        public CandidateJobOrderMap()
        {
            this.ToTable("CandidateJobOrder");
            this.HasKey(c =>c.Id);

        }
    }
}
