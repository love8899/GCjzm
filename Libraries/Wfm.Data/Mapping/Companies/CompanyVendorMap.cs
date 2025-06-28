using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Companies
{
    public class CompanyVendorMap : EntityTypeConfiguration<CompanyVendor>
    {
        public CompanyVendorMap()
        {
            this.ToTable("CompanyVendor");
            this.HasKey(co => co.Id);

            this.HasRequired(co => co.Company)
                .WithMany(c => c.CompanyVendors)
                .HasForeignKey(co => co.CompanyId);
            this.HasRequired(co => co.Vendor)
                .WithMany()
                .HasForeignKey(co => co.VendorId);
        }
    }
}
