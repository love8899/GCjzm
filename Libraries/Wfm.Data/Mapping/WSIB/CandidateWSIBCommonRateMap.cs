using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.WSIB;

namespace Wfm.Data.Mapping.WSIBS
{
    public class CandidateWSIBCommonRateMap : EntityTypeConfiguration<CandidateWSIBCommonRate>
    {
        public CandidateWSIBCommonRateMap()
        {
            this.ToTable("Candidate_WSIB_Common_Rate");
            this.HasKey(x => x.Id);
            this.HasRequired(x => x.Candidate).WithMany(c=>c.CandidateWSIBCommonRates).HasForeignKey(x => x.CandidateId);
            this.HasRequired(x => x.StateProvince).WithMany().HasForeignKey(x => x.ProvinceId);
        }
    }
}
