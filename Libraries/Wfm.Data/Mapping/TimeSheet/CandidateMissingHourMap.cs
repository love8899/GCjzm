using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Data.Mapping.TimeSheet
{
    public partial class CandidateMissingHourMap : EntityTypeConfiguration<CandidateMissingHour>
    {
        public CandidateMissingHourMap()
        {
            this.ToTable("CandidateMissingHour");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId).WillCascadeOnDelete(true);

            this.HasRequired(x => x.JobOrder)
                .WithMany()
                .HasForeignKey(x => x.JobOrderId).WillCascadeOnDelete(true);
        }
    }
}
