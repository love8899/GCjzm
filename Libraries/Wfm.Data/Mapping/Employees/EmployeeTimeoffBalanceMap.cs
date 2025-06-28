using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeTimeoffBalanceMap : EntityTypeConfiguration<EmployeeTimeoffBalance>
    {
        public EmployeeTimeoffBalanceMap()
        {
            this.ToTable("EmployeeTimeoffBalance");
            this.HasKey(e => e.Id);
            //
            this.HasRequired(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);
            this.HasRequired(x => x.EmployeeTimeoffType)
                .WithMany(x => x.EmployeeTimeoffBalances)
                .HasForeignKey(x => x.EmployeeTimeoffTypeId);
        }
    }
}
