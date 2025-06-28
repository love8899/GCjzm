using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyCandidateMap : EntityTypeConfiguration<CompanyCandidate>
    {
        public CompanyCandidateMap()
        {
            this.ToTable("CompanyCandidate");
            this.HasKey(cc => cc.Id);

            this.HasRequired(x => x.Company)
                .WithMany(y => y.CompanyCandidatePool)
                .HasForeignKey(x => x.CompanyId);
            this.HasRequired(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId);
            this.HasOptional(x => x.PreferredLocation)
                .WithMany()
                .HasForeignKey(x => x.PreferredLocationId);
        }
    }
}
