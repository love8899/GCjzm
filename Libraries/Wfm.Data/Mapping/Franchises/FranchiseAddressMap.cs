using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Data.Mapping.Franchises
{
    public partial class FranchiseAddressMap : EntityTypeConfiguration<FranchiseAddress>
    {

        public FranchiseAddressMap()
        {
            this.ToTable("FranchiseAddress");
            this.HasKey(fa => fa.Id);

            this.Property(fa => fa.LocationName).HasMaxLength(255);

            this.HasRequired(fa => fa.Franchise)
                .WithMany()
                .HasForeignKey(fa => fa.FranchiseId).WillCascadeOnDelete(false);
            this.HasRequired(fa => fa.Country)
                .WithMany()
                .HasForeignKey(fa => fa.CountryId).WillCascadeOnDelete(false);
            this.HasRequired(fa => fa.StateProvince)
                .WithMany()
                .HasForeignKey(fa => fa.StateProvinceId).WillCascadeOnDelete(false);
            this.HasRequired(fa => fa.City)
                .WithMany()
                .HasForeignKey(fa => fa.CityId).WillCascadeOnDelete(false);

        }
    }
}
