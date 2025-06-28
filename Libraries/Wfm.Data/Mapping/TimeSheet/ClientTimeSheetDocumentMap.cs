using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Data.Mapping.TimeSheet
{
    public class ClientTimeSheetDocumentMap : EntityTypeConfiguration<ClientTimeSheetDocument>
    {
        public ClientTimeSheetDocumentMap()
        {
            this.ToTable("ClientTimeSheetDocument");
            this.HasKey(c => c.Id);

            this.Property(c => c.FileName).HasMaxLength(100);
            this.HasRequired(c => c.JobOrder)
                .WithMany(j => j.ClientTimeSheetDocuments)
                .HasForeignKey(c => c.JobOrderId);
        }
    }
}
