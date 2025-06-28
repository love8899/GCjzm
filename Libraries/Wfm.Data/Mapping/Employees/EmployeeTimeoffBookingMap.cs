using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeTimeoffBookingMap : EntityTypeConfiguration<EmployeeTimeoffBooking>

    {
        public EmployeeTimeoffBookingMap()
        {
            this.ToTable("EmployeeTimeoffBooking");
            this.HasKey(e => e.Id);
            this.Property(x => x.Note)
                .HasMaxLength(1024);
            //
            this.HasRequired(x => x.Employee)
                .WithMany(y => y.EmployeeTimeoffBookings)
                .HasForeignKey(x => x.EmployeeId);
            this.HasOptional(x => x.EmployeeTimeoffType)
                .WithMany()
                .HasForeignKey(x => x.EmployeeTimeoffTypeId);
            this.HasOptional(x => x.EmployeeTimeoffBalance)
                .WithMany()
                .HasForeignKey(x => x.EmployeeTimeoffBalanceId);
            this.HasRequired(x => x.BookedByAccount)
                .WithMany()
                .HasForeignKey(x => x.BookedByAccountId);
            this.HasOptional(x => x.ApprovedByAccount)
                .WithMany()
                .HasForeignKey(x => x.ApprovedByAccountId);
        }
    }
}
