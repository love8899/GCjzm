using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.WSIBS;

namespace Wfm.Data.Mapping.WSIBS
{
    public class WSIBMap : EntityTypeConfiguration<WSIB>
    {
        public WSIBMap()
        {
            this.ToTable("WSIB");
            this.HasKey(c => c.Id);
            this.HasRequired(c => c.Province).WithMany().HasForeignKey(c => c.ProvinceId);
           
        }
    }
}
