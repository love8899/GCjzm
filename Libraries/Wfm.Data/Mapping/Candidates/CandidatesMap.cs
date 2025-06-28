using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidatesMap : EntityTypeConfiguration<Candidate>
    {
        public CandidatesMap() 
        {
            this.ToTable("Candidate");
            this.HasKey(c => c.Id);

            this.Property(c => c.CandidateGuid).IsRequired();
            this.Property(c => c.Username).HasMaxLength(255);
            this.Property(c => c.Email).HasMaxLength(255);

            this.Property(c => c.EmployeeId).HasMaxLength(255);

            this.Property(c => c.FirstName).HasMaxLength(255);
            this.Property(c => c.LastName).HasMaxLength(255);
            this.Property(c => c.MiddleName).HasMaxLength(255);

            this.Property(c => c.HomePhone).HasMaxLength(50);
            this.Property(c => c.MobilePhone).HasMaxLength(50);
            this.Property(c => c.EmergencyPhone).HasMaxLength(50);

            this.Ignore(u => u.PasswordFormat);

            this.Property(c => c.WorkPermit).HasMaxLength(30);
        }
    }
}
