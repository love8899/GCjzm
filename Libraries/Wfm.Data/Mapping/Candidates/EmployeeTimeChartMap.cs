using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class EmployeeTimeChartMap : EntityTypeConfiguration<EmployeeTimeChartHistory>
    {
        public EmployeeTimeChartMap()
        {
            this.ToTable("EmployeeTimeSheetHistory");
            this.HasKey(c => c.Id);
        }

    }
}
