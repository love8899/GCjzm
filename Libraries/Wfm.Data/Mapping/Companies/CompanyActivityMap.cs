using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyActivityMap : EntityTypeConfiguration<CompanyActivity>
    {
        public CompanyActivityMap()
        {
            this.ToTable("CompanyActivity");
            this.HasKey(x => x.Id);
            this.HasRequired(c => c.Company)
                .WithMany(c => c.CompanyActivities)
                .HasForeignKey(c => c.CompanyId);
            this.HasRequired(c => c.ActivityType)
                .WithMany()
                .HasForeignKey(c => c.ActivityTypeId);
        }
    }
}
