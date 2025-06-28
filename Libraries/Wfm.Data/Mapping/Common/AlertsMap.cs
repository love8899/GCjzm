using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public class AlertsMap : EntityTypeConfiguration<Alerts>
    {
        public AlertsMap()
        {
            this.ToTable("Alerts");
            this.HasKey(c => c.Id);

            this.Property(c => c.Message).HasMaxLength(200);
            this.HasRequired(c => c.Candidate)
                .WithMany(c => c.CandidateAlerts)
                .HasForeignKey(c => c.CandidateId);

            this.HasOptional(c => c.JobOrder)
                .WithMany()
                .HasForeignKey(c => c.JobOrderId);
        }
    }
}
