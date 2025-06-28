using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateJobOrderStatusMap : EntityTypeConfiguration<CandidateJobOrderStatus>
    {
        public CandidateJobOrderStatusMap()
        {
            this.ToTable("CandidateJobOrderStatus");
            this.HasKey(c => c.Id);

            this.Property(c => c.StatusName).HasMaxLength(255);
        }
    }
}
