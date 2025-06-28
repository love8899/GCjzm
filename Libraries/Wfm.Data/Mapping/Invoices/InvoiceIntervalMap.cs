using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Invoices;

namespace Wfm.Data.Mapping.Common
{
    public partial class InvoiceIntervalMap : EntityTypeConfiguration<InvoiceInterval>
    {
        public InvoiceIntervalMap()
        {
            this.ToTable("InvoiceInterval");

            this.HasKey(x => x.Id);

            this.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
