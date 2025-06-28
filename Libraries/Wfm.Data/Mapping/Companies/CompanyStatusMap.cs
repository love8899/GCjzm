using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyStatusMap : EntityTypeConfiguration<CompanyStatus>
    {
        public CompanyStatusMap()
        {
            this.ToTable("CompanyStatus");
            this.HasKey(x => x.Id);
        }
    }
}
