using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Data.Mapping.TimeSheet
{
    public class MissingHourDocumentMap : EntityTypeConfiguration<MissingHourDocument>
    {
        public MissingHourDocumentMap()
        {
            this.ToTable("MissingHourDocument");
            this.HasKey(x => x.Id);

            this.Property(x => x.FileName).HasMaxLength(255);

            this.HasRequired(x => x.CandidateMissingHour)
                .WithMany(y => y.MissingHourDocuments)
                .HasForeignKey(x => x.CandidateMissingHourId);
        }
    }
}
