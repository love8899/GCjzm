using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Data.Mapping.JobOrders
{
    public class JobOrderOvertimeRuleMap : EntityTypeConfiguration<JobOrderOvertimeRule>
    {
        public JobOrderOvertimeRuleMap()
        {
            this.ToTable("JobOrderOvertimeRule");
            this.HasKey(jo => jo.Id);

            this.HasRequired(jo => jo.JobOrder)
                .WithMany(j => j.JobOrderOvertimeRules)
                .HasForeignKey(jo => jo.JobOrderId);
        }
    }
}
