using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;


namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeTimeoffTypeMap : EntityTypeConfiguration<EmployeeTimeoffType>
    {
        public EmployeeTimeoffTypeMap()
        {
            this.ToTable("EmployeeTimeoffType");
            this.HasKey(e => e.Id);
            //
            this.Property(x => x.Name)
                .HasMaxLength(50);
            this.Property(x => x.Description)
                .HasMaxLength(500);
            this.HasRequired(x => x.EmployeeType).WithMany().HasForeignKey(x => x.EmployeeTypeId);
        }
    }
}
