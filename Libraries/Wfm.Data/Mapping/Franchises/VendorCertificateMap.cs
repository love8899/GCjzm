using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Data.Mapping.Franchises
{
    class VendorCertificateMap : EntityTypeConfiguration<VendorCertificate>
    {
        public VendorCertificateMap()
        {
            this.ToTable("VendorCertificate");
            this.HasKey(x => x.Id);
            this.Property(x => x.CertificateGuid).IsRequired();
            this.HasRequired(x => x.Vendor).WithMany(x=>x.VendorCertificates).HasForeignKey(x => x.FranchiseId);
        }
    }
}
