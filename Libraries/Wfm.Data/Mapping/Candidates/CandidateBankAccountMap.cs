using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateBankAccountMap : EntityTypeConfiguration<CandidateBankAccount>
    {

        public CandidateBankAccountMap()
        {
            this.ToTable("CandidateBankAccount");
            this.HasKey(c => c.Id);
            this.HasRequired(x => x.Candidate).WithMany(x => x.CandidateBankAccounts).HasForeignKey(x => x.CandidateId);
            this.Property(c => c.InstitutionNumber).HasMaxLength(255);
            this.Property(c => c.TransitNumber).HasMaxLength(255);
            this.Property(c => c.AccountNumber).HasMaxLength(255);
        }
    }
}
