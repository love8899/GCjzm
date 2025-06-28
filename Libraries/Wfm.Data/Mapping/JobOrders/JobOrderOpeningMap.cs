using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public class JobOrderOpeningMap : EntityTypeConfiguration<JobOrderOpening>
    {
        public JobOrderOpeningMap()
        {
            this.ToTable("JobOrderOpening");
            this.HasKey(jo => jo.Id);

            this.HasRequired(x => x.JobOrder)
                .WithMany(y => y.JobOrderOpenings)
                .HasForeignKey(x => x.JobOrderId);
        }
    }
}
