using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateAddressMap : EntityTypeConfiguration<CandidateAddress>
    {

        public CandidateAddressMap()
        {
            this.ToTable("CandidateAddress");
            this.HasKey(c => c.Id);

            this.HasRequired(ca => ca.AddressType)
                .WithMany()
                .HasForeignKey(ca => ca.AddressTypeId).WillCascadeOnDelete(false);
        }
    }
}
