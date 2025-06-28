using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class FranchiseBankInfoMap : EntityTypeConfiguration<FranchiseBankInfo>
    {

        public FranchiseBankInfoMap()
        {
            this.ToTable("FranchiseBankInfo");
            this.HasKey(c => c.Id);

            this.Property(c => c.ClientNumber).HasMaxLength(255);


            this.Property(c => c.InstitutionNumber).HasMaxLength(255);
            this.Property(c => c.TransitNumber).HasMaxLength(255);

        }
    }
}
