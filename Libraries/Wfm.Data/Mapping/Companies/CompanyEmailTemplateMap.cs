using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyEmailTemplateMap : EntityTypeConfiguration<CompanyEmailTemplate>
    {
        public CompanyEmailTemplateMap ()
	    {
            this.ToTable("CompanyEmailTemplate");
            this.HasKey(x => x.Id);
            this.HasRequired(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId);
            this.HasRequired(c => c.CompanyLocation)
                .WithMany()
                .HasForeignKey(c => c.CompanyLocationId);
            this.HasRequired(c => c.CompanyDepartment)
                .WithMany()
                .HasForeignKey(c => c.CompanyDepartmentId);
	    }
       

    }
}
