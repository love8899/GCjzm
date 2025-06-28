using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.ClockTime;

namespace Wfm.Data.Mapping.TimeClocks
{
   public partial class CandidateClockTimeMap : EntityTypeConfiguration<CandidateClockTime>
    {
       public CandidateClockTimeMap()
       {
           this.ToTable("CandidateClockTime");
           this.HasKey(cct => cct.Id);

           this.Property(cct => cct.SmartCardUid).HasMaxLength(255);
           this.Property(cct => cct.ClockDeviceUid).HasMaxLength(255);


           this.Ignore(c => c.CandidateClockTimeStatus);
       }
    }
}
