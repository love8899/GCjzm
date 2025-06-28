using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.ClockTime;

namespace Wfm.Data.Mapping.TimeClocks
{
    public partial class CandidateSmartCardMap : EntityTypeConfiguration<CandidateSmartCard>
    {
         public CandidateSmartCardMap()
       {
           this.ToTable("CandidateSmartCard");
           this.HasKey(csc => csc.Id);

           this.Property(csc => csc.SmartCardUid).IsRequired().HasMaxLength(255);
           this.Property(csc => csc.CandidateSmartCardGuid).IsRequired();
           this.HasRequired(c => c.Candidate).WithMany(c => c.SmartCards).HasForeignKey(x=>x.CandidateId);
       }
    }
}
