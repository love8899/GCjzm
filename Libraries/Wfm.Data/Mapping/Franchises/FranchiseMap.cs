using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Data.Mapping.Franchises
{
    public partial class FranchiseMap : EntityTypeConfiguration<Franchise>
    {

        public FranchiseMap()
        {
            this.ToTable("Franchise");
            this.HasKey(c => c.Id);

            this.Property(u => u.FranchiseGuid).IsRequired();
            this.Property(c => c.FranchiseName).IsRequired().HasMaxLength(255);
        }
    }
}
