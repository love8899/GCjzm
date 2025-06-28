using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Data.Mapping.TimeSheet
{
    public partial class CandidateWorkTimeMap : EntityTypeConfiguration<CandidateWorkTime>
    {
        public CandidateWorkTimeMap()
        {
            this.ToTable("CandidateWorkTime");
            this.HasKey(c => c.Id);

            this.Property(c => c.ClockTimeInMinutes).HasPrecision(18, 2);
            this.Property(c => c.AdjustmentInMinutes).HasPrecision(18, 2);


            this.Ignore(c => c.CandidateWorkTimeStatus);
        }
    }
}
