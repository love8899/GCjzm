using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class EmployeeScheduleValidationResultMap : EntityTypeConfiguration<EmployeeScheduleValidationResult>
    {
        public EmployeeScheduleValidationResultMap()
        {
            this.ToTable("EmployeeScheduleValidationResult");
            this.HasKey(a => a.Id);
            this.Property(a => a.ValidationResultMessage).HasMaxLength(1024);

            this.HasRequired(c => c.EmployeeSchedule)
                .WithMany(x => x.ValidationResults)
                .HasForeignKey(c => c.EmployeeScheduleId);
            this.HasOptional(c => c.EmployeeScheduleDaily)
                .WithMany(x => x.ValidationResults)
                .HasForeignKey(c => c.EmployeeScheduleDailyId);
            this.HasOptional(c => c.OverridenBy)
                .WithMany()
                .HasForeignKey(c => c.OverridenById);
        }
    }
}

