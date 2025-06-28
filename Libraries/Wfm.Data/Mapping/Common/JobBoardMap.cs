using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public class JobBoardMap : EntityTypeConfiguration<JobBoard>
    {
        public JobBoardMap()
        {
            this.ToTable("JobBoard");
            this.HasKey(c => c.Id);
        }
    }
}
