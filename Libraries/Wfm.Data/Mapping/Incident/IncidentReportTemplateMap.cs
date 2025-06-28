using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Incident;

namespace Wfm.Data.Mapping.Incident
{
    public class IncidentReportTemplateMap : EntityTypeConfiguration<IncidentReportTemplate>
    {
        public IncidentReportTemplateMap()
        {
            this.ToTable("IncidentReportTemplate");
            this.HasKey(e => e.Id);

            this.Property(e => e.FileName).HasMaxLength(80);
            this.Property(e => e.Note).HasMaxLength(1000);

            this.HasRequired(e => e.IncidentCategory)
                .WithMany(e => e.Templates)
                .HasForeignKey(e => e.IncidentCategoryId).WillCascadeOnDelete(false);
            this.HasOptional(e => e.Franchise)
                .WithMany()
                .HasForeignKey(e => e.FranchiseId).WillCascadeOnDelete(false);
        }
    }
}

