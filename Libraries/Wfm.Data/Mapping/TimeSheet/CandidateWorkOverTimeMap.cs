using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Data.Mapping.TimeSheet
{
    public class CandidateWorkOverTimeMap: EntityTypeConfiguration<CandidateWorkOverTime>
    {
        public CandidateWorkOverTimeMap()
        {
            this.ToTable("CandidateWorkOverTime");
            this.HasKey(c => c.Id);

            this.Property(c => c.OvertimeHours).HasPrecision(5, 2);

            this.Property(c => c.OvertimePayRate).HasPrecision(5, 2);

            this.HasRequired(c => c.Candidate)
                .WithMany()
                .HasForeignKey(c => c.CandidateId);
            
            this.HasRequired(c => c.JobOrder)
                .WithMany()
                .HasForeignKey(c => c.JobOrderId);

            this.HasRequired(c => c.OvertimeType)
                .WithMany()
                .HasForeignKey(c => c.OvertimeTypeId);
        }
    }
}
