using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateDirectHireStatusHistoryMap : EntityTypeConfiguration<CandidateDirectHireStatusHistory>
    {
        public CandidateDirectHireStatusHistoryMap()
        {
            this.ToTable("CandidateDirectHireStatusHistory");
            this.HasKey(c => c.Id);

        }
    }
}
