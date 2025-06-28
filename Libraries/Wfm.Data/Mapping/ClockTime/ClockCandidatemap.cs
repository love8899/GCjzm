using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Data.Mapping.TimeClocks
{
    public class ClockCandidateMap : EntityTypeConfiguration<ClockCandidate>
    {
        public ClockCandidateMap()
        {
            this.ToTable("ClockCandidate");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.ClockDevice)
                .WithMany()
                .HasForeignKey(x => x.ClockDeviceId);
        }
    }
}
