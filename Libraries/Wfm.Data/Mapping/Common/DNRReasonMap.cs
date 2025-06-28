using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public class DNRReasonMap : EntityTypeConfiguration<DNRReason>
    {
        public DNRReasonMap()
        {
            this.ToTable("DNRReason");
            this.HasKey(c => c.Id);

            this.Property(c => c.Reason).IsRequired().HasMaxLength(255);
        }
    }
}
