using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeAvailabilityMap : EntityTypeConfiguration<EmployeeAvailability>
    {
        public EmployeeAvailabilityMap()
        {
            this.ToTable("EmployeeAvailability");
            this.HasKey(e => e.Id);
            //
            this.HasRequired(x => x.Employee)
                .WithMany(x => x.EmployeeAvailabilities)
                .HasForeignKey(x => x.EmployeeId);
        }
    }
}
