using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateAvailabilityMap : EntityTypeConfiguration<CandidateAvailability>
    {
        public CandidateAvailabilityMap()
        {
            this.ToTable("CandidateAvailability");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId);

            this.HasRequired(x => x.Shift)
                .WithMany()
                .HasForeignKey(x => x.ShiftId);

            this.Property(x => x.Note).HasMaxLength(20);
        }
    }
}
