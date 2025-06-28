using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class SalutationMap : EntityTypeConfiguration<Salutation>
    {
        public SalutationMap()
        {
            this.ToTable("Salutation");
            this.HasKey(c => c.Id);

            this.Property(c => c.SalutationName).IsRequired().HasMaxLength(255);

            this.HasRequired(s => s.Gender)
                .WithMany()
                .HasForeignKey(s => s.GenderId);
        }
    }
}
