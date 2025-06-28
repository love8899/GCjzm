using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Tests;

namespace Wfm.Data.Mapping.Tests
{
    public class CandidateTestLinkMap : EntityTypeConfiguration<CandidateTestLink>
    {
        public CandidateTestLinkMap()
        {
            this.ToTable("CandidateTestLink");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.TestCategory).WithMany().HasForeignKey(c => c.TestCategoryId);
            this.HasRequired(c => c.Candidate).WithMany().HasForeignKey(c => c.CandidateId);
        }
    }
}
