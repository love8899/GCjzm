using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateTestResultMap : EntityTypeConfiguration<CandidateTestResult>
    {
        public CandidateTestResultMap()
        {
            this.ToTable("CandidateTestResult");
            this.HasKey(c => c.Id);


            this.HasRequired(ctr => ctr.Candidate)
                .WithMany(c => c.CandidateTestResults)
                .HasForeignKey(ctr => ctr.CandidateId);
        }
    }
}
