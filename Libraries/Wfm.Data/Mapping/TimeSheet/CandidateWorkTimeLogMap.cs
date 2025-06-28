using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Data.Mapping.TimeSheet
{
    public partial class CandidateWorkTimeLogMap : EntityTypeConfiguration<CandidateWorkTimeLog>
    {
        public CandidateWorkTimeLogMap()
        {
            this.ToTable("CandidateWorkTimeLog");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.CandidateWorkTime)
                .WithMany(y => y.CandidateWorkTimeChanges)
                .HasForeignKey(x => x.CandidateWorkTimeId);
        }
    }
}
