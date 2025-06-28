using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeTypeMap : EntityTypeConfiguration<EmployeeType>
    {
        public EmployeeTypeMap()
        {
            this.ToTable("Employee_Type");
            this.HasKey(e => e.Id);
            //
            this.Property(x => x.Name)
                .HasMaxLength(50);
            this.Property(x => x.Code)
                .HasMaxLength(50);
        }
    }
}
