using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Incident;

namespace Wfm.Data.Mapping.Incident
{
    public class IncidentReportFileMap : EntityTypeConfiguration<IncidentReportFile>
    {
        public IncidentReportFileMap()
        {
            this.ToTable("IncidentReportFile");
            this.HasKey(e => e.Id);

            this.Property(e => e.FileName).HasMaxLength(80);

            this.HasRequired(e => e.IncidentReport)
                .WithMany(e => e.ReportFiles)
                .HasForeignKey(e => e.IncidentReportId).WillCascadeOnDelete(false);
        }
    }
}
