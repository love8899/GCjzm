using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Incident;

namespace Wfm.Data.Mapping.Incident
{
    public class IncidentCategoryMap : EntityTypeConfiguration<IncidentCategory>
    {
        public IncidentCategoryMap()
        {
            this.ToTable("IncidentCategory");
            this.HasKey(e => e.Id);

            this.Property(e => e.Description).HasMaxLength(250);

            this.HasOptional(e => e.Franchise)
                .WithMany()
                .HasForeignKey(e => e.FranchiseId).WillCascadeOnDelete(false);
        }
    }
}
