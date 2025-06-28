using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidatePictureMap : EntityTypeConfiguration<CandidatePicture>
    {
        public CandidatePictureMap()
        {
            //this.ToTable("Candidate_Picture_Mapping");
            //this.HasKey(pp => pp.Id);
            
            //this.HasRequired(pp => pp.Picture)
            //    .WithMany(p => p.CandidatePictures)
            //    .HasForeignKey(pp => pp.PictureId);


            //this.HasRequired(pp => pp.Candidate)
            //    .WithMany(p => p.CandidatePictures)
            //    .HasForeignKey(pp => pp.CandidateId);



            this.ToTable("CandidatePicture");
            this.HasKey(cp => cp.Id);

            this.Property(cp => cp.FilePath).HasMaxLength(1000);
            this.Property(cp => cp.MimeType).HasMaxLength(255);

        }
    }
}