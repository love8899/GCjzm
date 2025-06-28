using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyAttachmentMap : EntityTypeConfiguration<CompanyAttachment>
    {
        public CompanyAttachmentMap()
        {
            this.ToTable("CompanyAttachment");
            this.HasKey(x => x.Id);
            this.HasRequired(x => x.Company).WithMany(x => x.CompanyAttachments).HasForeignKey(x => x.CompanyId);
            this.HasRequired(x => x.AttachmentType).WithMany().HasForeignKey(x => x.AttachmentTypeId);
        }
    }
}
