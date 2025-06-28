using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateJobOrderStatusHistoryMap : EntityTypeConfiguration<CandidateJobOrderStatusHistory>
    {
        public CandidateJobOrderStatusHistoryMap()
        {
            this.ToTable("CandidateJobOrderStatusHistory");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.Account)
                .WithMany()
                .HasForeignKey(c => c.EnteredBy);
            this.HasOptional(c => c.CandidateJobOrderStatus)
                .WithMany()
                .HasForeignKey(c => c.Status);
        }
    }
}
