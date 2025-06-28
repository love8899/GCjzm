using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public class CandidateAppiedJobsMap : EntityTypeConfiguration<CandidateAppliedJobs>
    {
        public CandidateAppiedJobsMap()
        {
            this.ToTable("CandidateAppliedJobs");
            this.HasKey(c => c.Id);
            this.HasRequired(c => c.Candidate).WithMany(x => x.AppliedJobs).HasForeignKey(x => x.CandidateId);
            this.HasRequired(c => c.JobOrder).WithMany(x=>x.CandidateApplied).HasForeignKey(x => x.JobOrderId);
            
        }
    }
}
