using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Incident;

namespace Wfm.Data.Mapping.Incident
{
    public class IncidentReportMap : EntityTypeConfiguration<IncidentReport>
    {
        public IncidentReportMap()
        {
            this.ToTable("IncidentReport");
            this.HasKey(e => e.Id);

            this.HasRequired(e => e.Candidate)
                .WithMany(e => e.IncidentReports)
                .HasForeignKey(e => e.CandidateId).WillCascadeOnDelete(false);
            this.HasRequired(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId).WillCascadeOnDelete(false);
            this.HasRequired(e => e.IncidentCategory)
                .WithMany()
                .HasForeignKey(e => e.IncidentCategoryId).WillCascadeOnDelete(false);
            this.HasRequired(e => e.ReportedByAccount)
                .WithMany()
                .HasForeignKey(e => e.ReportedByAccountId).WillCascadeOnDelete(false);
            this.HasOptional(e => e.JobOrder)
                .WithMany()
                .HasForeignKey(e => e.JobOrderId).WillCascadeOnDelete(false);
            this.HasOptional(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId).WillCascadeOnDelete(false);
        }
    }
}
